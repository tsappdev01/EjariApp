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
using DIP.Zones;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.Zones
{

    [Authorize(DIPPermissions.Zones.Default)]
    public partial class ZonesAppService : ApplicationService, IZonesAppService
    {
        private readonly IDistributedCache<ZoneExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IZoneRepository _zoneRepository;
        private readonly ZoneManager _zoneManager;

        public ZonesAppService(IZoneRepository zoneRepository, ZoneManager zoneManager, IDistributedCache<ZoneExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _zoneRepository = zoneRepository;
            _zoneManager = zoneManager;
        }

        public virtual async Task<PagedResultDto<ZoneDto>> GetListAsync(GetZonesInput input)
        {
            var totalCount = await _zoneRepository.GetCountAsync(input.FilterText, input.TitleEn, input.TitleAR, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaTitleAr, input.MetaDescriptionAr, input.Slug, input.SummaryEn, input.SummaryAr, input.Image, input.HeaderImage, input.OrderMin, input.OrderMax, input.IsFeature, input.IsActive);
            var items = await _zoneRepository.GetListAsync(input.FilterText, input.TitleEn, input.TitleAR, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaTitleAr, input.MetaDescriptionAr, input.Slug, input.SummaryEn, input.SummaryAr, input.Image, input.HeaderImage, input.OrderMin, input.OrderMax, input.IsFeature, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<ZoneDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Zone>, List<ZoneDto>>(items)
            };
        }

        public virtual async Task<ZoneDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Zone, ZoneDto>(await _zoneRepository.GetAsync(id));
        }

        [Authorize(DIPPermissions.Zones.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _zoneRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.Zones.Create)]
        public virtual async Task<ZoneDto> CreateAsync(ZoneCreateDto input)
        {

            var zone = await _zoneManager.CreateAsync(
            input.TitleEn, input.TitleAR, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaTitleAr, input.MetaDescriptionAr, input.Slug, input.SummaryEn, input.SummaryAr, input.Image, input.HeaderImage, input.Order, input.IsFeature, input.IsActive
            );

            return ObjectMapper.Map<Zone, ZoneDto>(zone);
        }

        [Authorize(DIPPermissions.Zones.Edit)]
        public virtual async Task<ZoneDto> UpdateAsync(Guid id, ZoneUpdateDto input)
        {

            var zone = await _zoneManager.UpdateAsync(
            id,
            input.TitleEn, input.TitleAR, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaTitleAr, input.MetaDescriptionAr, input.Slug, input.SummaryEn, input.SummaryAr, input.Image, input.HeaderImage, input.Order, input.IsFeature, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Zone, ZoneDto>(zone);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(ZoneExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _zoneRepository.GetListAsync(input.FilterText, input.TitleEn, input.TitleAR, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaTitleAr, input.MetaDescriptionAr, input.Slug, input.SummaryEn, input.SummaryAr, input.Image, input.HeaderImage, input.OrderMin, input.OrderMax, input.IsFeature, input.IsActive);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<Zone>, List<ZoneExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Zones.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new ZoneExcelDownloadTokenCacheItem { Token = token },
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