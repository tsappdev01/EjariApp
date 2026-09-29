using DIP.Shared;
using DIP.TimeLineCategories;
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
using DIP.TimeLines;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.TimeLines
{

    [Authorize(DIPPermissions.TimeLines.Default)]
    public class TimeLinesAppService : ApplicationService, ITimeLinesAppService
    {
        private readonly IDistributedCache<TimeLineExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly ITimeLineRepository _timeLineRepository;
        private readonly TimeLineManager _timeLineManager;
        private readonly IRepository<TimeLineCategory, Guid> _timeLineCategoryRepository;

        public TimeLinesAppService(ITimeLineRepository timeLineRepository, TimeLineManager timeLineManager, IDistributedCache<TimeLineExcelDownloadTokenCacheItem, string> excelDownloadTokenCache, IRepository<TimeLineCategory, Guid> timeLineCategoryRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _timeLineRepository = timeLineRepository;
            _timeLineManager = timeLineManager; _timeLineCategoryRepository = timeLineCategoryRepository;
        }

        public virtual async Task<PagedResultDto<TimeLineWithNavigationPropertiesDto>> GetListAsync(GetTimeLinesInput input)
        {
            var totalCount = await _timeLineRepository.GetCountAsync(input.FilterText, input.TitleEn, input.TitleAr, input.DescriptionEn, input.DescriptionAr, input.Image, input.TimeLineDateMin, input.TimeLineDateMax, input.OrderMin, input.OrderMax, input.IsActive, input.TimeLineCategoryId);
            var items = await _timeLineRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TitleEn, input.TitleAr, input.DescriptionEn, input.DescriptionAr, input.Image, input.TimeLineDateMin, input.TimeLineDateMax, input.OrderMin, input.OrderMax, input.IsActive, input.TimeLineCategoryId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<TimeLineWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<TimeLineWithNavigationProperties>, List<TimeLineWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<TimeLineWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<TimeLineWithNavigationProperties, TimeLineWithNavigationPropertiesDto>
                (await _timeLineRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<TimeLineDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<TimeLine, TimeLineDto>(await _timeLineRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetTimeLineCategoryLookupAsync(LookupRequestDto input)
        {
            var query = (await _timeLineCategoryRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.TitleEn != null &&
                         x.TitleEn.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<TimeLineCategory>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<TimeLineCategory>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(DIPPermissions.TimeLines.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _timeLineRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.TimeLines.Create)]
        public virtual async Task<TimeLineDto> CreateAsync(TimeLineCreateDto input)
        {
            if (input.TimeLineCategoryId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["TimeLineCategory"]]);
            }

            var timeLine = await _timeLineManager.CreateAsync(
            input.TimeLineCategoryId, input.TitleEn, input.TitleAr, input.DescriptionEn, input.DescriptionAr, input.Image, input.Order, input.IsActive, input.TimeLineDate
            );

            return ObjectMapper.Map<TimeLine, TimeLineDto>(timeLine);
        }

        [Authorize(DIPPermissions.TimeLines.Edit)]
        public virtual async Task<TimeLineDto> UpdateAsync(Guid id, TimeLineUpdateDto input)
        {
            if (input.TimeLineCategoryId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["TimeLineCategory"]]);
            }

            var timeLine = await _timeLineManager.UpdateAsync(
            id,
            input.TimeLineCategoryId, input.TitleEn, input.TitleAr, input.DescriptionEn, input.DescriptionAr, input.Image, input.Order, input.IsActive, input.TimeLineDate, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<TimeLine, TimeLineDto>(timeLine);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(TimeLineExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var timeLines = await _timeLineRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TitleEn, input.TitleAr, input.DescriptionEn, input.DescriptionAr, input.Image, input.TimeLineDateMin, input.TimeLineDateMax, input.OrderMin, input.OrderMax, input.IsActive);
            var items = timeLines.Select(item => new
            {
                TitleEn = item.TimeLine.TitleEn,
                TitleAr = item.TimeLine.TitleAr,
                DescriptionEn = item.TimeLine.DescriptionEn,
                DescriptionAr = item.TimeLine.DescriptionAr,
                Image = item.TimeLine.Image,
                TimeLineDate = item.TimeLine.TimeLineDate,
                Order = item.TimeLine.Order,
                IsActive = item.TimeLine.IsActive,

                TimeLineCategory = item.TimeLineCategory?.TitleEn,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "TimeLines.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new TimeLineExcelDownloadTokenCacheItem { Token = token },
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