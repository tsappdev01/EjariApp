using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using DIP.Permissions;
using DIP.TimeLineCategories;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.TimeLineCategories
{

    [Authorize(DIPPermissions.TimeLineCategories.Default)]
    public partial class TimeLineCategoriesAppService : ApplicationService, ITimeLineCategoriesAppService
    {
        private readonly IDistributedCache<TimeLineCategoryExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly ITimeLineCategoryRepository _timeLineCategoryRepository;
        private readonly TimeLineCategoryManager _timeLineCategoryManager;

        public TimeLineCategoriesAppService(ITimeLineCategoryRepository timeLineCategoryRepository, TimeLineCategoryManager timeLineCategoryManager, IDistributedCache<TimeLineCategoryExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _timeLineCategoryRepository = timeLineCategoryRepository;
            _timeLineCategoryManager = timeLineCategoryManager;
        }

        public virtual async Task<PagedResultDto<TimeLineCategoryDto>> GetListAsync(GetTimeLineCategoriesInput input)
        {
            var totalCount = await _timeLineCategoryRepository.GetCountAsync(input.FilterText, input.TitleEn, input.TitleAr, input.OrderMin, input.OrderMax, input.IsActive);
            var items = await _timeLineCategoryRepository.GetListAsync(input.FilterText, input.TitleEn, input.TitleAr, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<TimeLineCategoryDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<TimeLineCategory>, List<TimeLineCategoryDto>>(items)
            };
        }

        public virtual async Task<TimeLineCategoryDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<TimeLineCategory, TimeLineCategoryDto>(await _timeLineCategoryRepository.GetAsync(id));
        }

        [Authorize(DIPPermissions.TimeLineCategories.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _timeLineCategoryRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.TimeLineCategories.Create)]
        public virtual async Task<TimeLineCategoryDto> CreateAsync(TimeLineCategoryCreateDto input)
        {

            var timeLineCategory = await _timeLineCategoryManager.CreateAsync(
            input.TitleEn, input.TitleAr, input.Order, input.IsActive
            );

            return ObjectMapper.Map<TimeLineCategory, TimeLineCategoryDto>(timeLineCategory);
        }

        [Authorize(DIPPermissions.TimeLineCategories.Edit)]
        public virtual async Task<TimeLineCategoryDto> UpdateAsync(Guid id, TimeLineCategoryUpdateDto input)
        {

            var timeLineCategory = await _timeLineCategoryManager.UpdateAsync(
            id,
            input.TitleEn, input.TitleAr, input.Order, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<TimeLineCategory, TimeLineCategoryDto>(timeLineCategory);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(TimeLineCategoryExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _timeLineCategoryRepository.GetListAsync(input.FilterText, input.TitleEn, input.TitleAr, input.OrderMin, input.OrderMax, input.IsActive);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<TimeLineCategory>, List<TimeLineCategoryExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "TimeLineCategories.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new TimeLineCategoryExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}