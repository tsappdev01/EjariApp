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
using DIP.LastEventss;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.LastEventss
{

    [Authorize(DIPPermissions.LastEventss.Default)]
    public partial class LastEventssAppService : ApplicationService, ILastEventssAppService
    {
        private readonly IDistributedCache<LastEventsExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly ILastEventsRepository _lastEventsRepository;
        private readonly LastEventsManager _lastEventsManager;

        public LastEventssAppService(ILastEventsRepository lastEventsRepository, LastEventsManager lastEventsManager, IDistributedCache<LastEventsExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _lastEventsRepository = lastEventsRepository;
            _lastEventsManager = lastEventsManager;
        }

        public virtual async Task<PagedResultDto<LastEventsDto>> GetListAsync(GetLastEventssInput input)
        {
            var totalCount = await _lastEventsRepository.GetCountAsync(input.FilterText, input.TitleAr, input.TitleEn, input.MetaTitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaDescriptionAr, input.IsFeatured, input.Slug, input.Image, input.HeaderImage, input.DescriptionAr, input.DescriptionEn, input.SummaryEn, input.SummaryAr, input.OrderMin, input.OrderMax, input.StartDateMin, input.StartDateMax, input.EndDateMin, input.EndDateMax, input.IsActive, input.LocationAr, input.LocationEn);
            var items = await _lastEventsRepository.GetListAsync(input.FilterText, input.TitleAr, input.TitleEn, input.MetaTitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaDescriptionAr, input.IsFeatured, input.Slug, input.Image, input.HeaderImage, input.DescriptionAr, input.DescriptionEn, input.SummaryEn, input.SummaryAr, input.OrderMin, input.OrderMax, input.StartDateMin, input.StartDateMax, input.EndDateMin, input.EndDateMax, input.IsActive, input.LocationAr, input.LocationEn, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<LastEventsDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<LastEvents>, List<LastEventsDto>>(items)
            };
        }

        public virtual async Task<LastEventsDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<LastEvents, LastEventsDto>(await _lastEventsRepository.GetAsync(id));
        }

        [Authorize(DIPPermissions.LastEventss.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _lastEventsRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.LastEventss.Create)]
        public virtual async Task<LastEventsDto> CreateAsync(LastEventsCreateDto input)
        {

            var lastEvents = await _lastEventsManager.CreateAsync(
            input.TitleAr, input.TitleEn, input.MetaTitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaDescriptionAr, input.IsFeatured, input.Slug, input.Image, input.HeaderImage, input.DescriptionAr, input.DescriptionEn, input.SummaryEn, input.SummaryAr, input.Order, input.StartDate, input.EndDate, input.IsActive, input.LocationAr, input.LocationEn
            );

            return ObjectMapper.Map<LastEvents, LastEventsDto>(lastEvents);
        }

        [Authorize(DIPPermissions.LastEventss.Edit)]
        public virtual async Task<LastEventsDto> UpdateAsync(Guid id, LastEventsUpdateDto input)
        {

            var lastEvents = await _lastEventsManager.UpdateAsync(
            id,
            input.TitleAr, input.TitleEn, input.MetaTitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaDescriptionAr, input.IsFeatured, input.Slug, input.Image, input.HeaderImage, input.DescriptionAr, input.DescriptionEn, input.SummaryEn, input.SummaryAr, input.Order, input.StartDate, input.EndDate, input.IsActive, input.LocationAr, input.LocationEn, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<LastEvents, LastEventsDto>(lastEvents);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(LastEventsExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _lastEventsRepository.GetListAsync(input.FilterText, input.TitleAr, input.TitleEn, input.MetaTitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaDescriptionAr, input.IsFeatured, input.Slug, input.Image, input.HeaderImage, input.DescriptionAr, input.DescriptionEn, input.SummaryEn, input.SummaryAr, input.OrderMin, input.OrderMax, input.StartDateMin, input.StartDateMax, input.EndDateMin, input.EndDateMax, input.IsActive, input.LocationAr, input.LocationEn);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<LastEvents>, List<LastEventsExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "LastEventss.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new LastEventsExcelDownloadTokenCacheItem { Token = token },
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