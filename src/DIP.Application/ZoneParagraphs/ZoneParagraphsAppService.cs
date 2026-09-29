using DIP.Shared;
using DIP.Zones;
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
using DIP.ZoneParagraphs;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.ZoneParagraphs
{

    [Authorize(DIPPermissions.ZoneParagraphs.Default)]
    public class ZoneParagraphsAppService : ApplicationService, IZoneParagraphsAppService
    {
        private readonly IDistributedCache<ZoneParagraphExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IZoneParagraphRepository _zoneParagraphRepository;
        private readonly ZoneParagraphManager _zoneParagraphManager;
        private readonly IRepository<Zone, Guid> _zoneRepository;

        public ZoneParagraphsAppService(IZoneParagraphRepository zoneParagraphRepository, ZoneParagraphManager zoneParagraphManager, IDistributedCache<ZoneParagraphExcelDownloadTokenCacheItem, string> excelDownloadTokenCache, IRepository<Zone, Guid> zoneRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _zoneParagraphRepository = zoneParagraphRepository;
            _zoneParagraphManager = zoneParagraphManager; _zoneRepository = zoneRepository;
        }

        public virtual async Task<PagedResultDto<ZoneParagraphWithNavigationPropertiesDto>> GetListAsync(GetZoneParagraphsInput input)
        {
            var totalCount = await _zoneParagraphRepository.GetCountAsync(input.FilterText, input.TitleEn, input.TitleAr, input.SubTilteEn, input.SubTitleAr, input.DescriptionEn, input.DescriptionAr, input.OrderMin, input.OrderMax, input.IsActive, input.ZoneId);
            var items = await _zoneParagraphRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TitleEn, input.TitleAr, input.SubTilteEn, input.SubTitleAr, input.DescriptionEn, input.DescriptionAr, input.OrderMin, input.OrderMax, input.IsActive, input.ZoneId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<ZoneParagraphWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<ZoneParagraphWithNavigationProperties>, List<ZoneParagraphWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<ZoneParagraphWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<ZoneParagraphWithNavigationProperties, ZoneParagraphWithNavigationPropertiesDto>
                (await _zoneParagraphRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<ZoneParagraphDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<ZoneParagraph, ZoneParagraphDto>(await _zoneParagraphRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetZoneLookupAsync(LookupRequestDto input)
        {
            var query = (await _zoneRepository.GetQueryableAsync())
                .OrderBy(x=>x.Order)
                .Where(x=>x.IsActive)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.TitleEn != null &&
                         x.TitleEn.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Zone>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Zone>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(DIPPermissions.ZoneParagraphs.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _zoneParagraphRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.ZoneParagraphs.Create)]
        public virtual async Task<ZoneParagraphDto> CreateAsync(ZoneParagraphCreateDto input)
        {
            if (input.ZoneId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Zone"]]);
            }

            var zoneParagraph = await _zoneParagraphManager.CreateAsync(
            input.ZoneId, input.TitleEn, input.TitleAr, input.SubTilteEn, input.SubTitleAr, input.DescriptionEn, input.DescriptionAr, input.Order, input.IsActive
            );

            return ObjectMapper.Map<ZoneParagraph, ZoneParagraphDto>(zoneParagraph);
        }

        [Authorize(DIPPermissions.ZoneParagraphs.Edit)]
        public virtual async Task<ZoneParagraphDto> UpdateAsync(Guid id, ZoneParagraphUpdateDto input)
        {
            if (input.ZoneId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Zone"]]);
            }

            var zoneParagraph = await _zoneParagraphManager.UpdateAsync(
            id,
            input.ZoneId, input.TitleEn, input.TitleAr, input.SubTilteEn, input.SubTitleAr, input.DescriptionEn, input.DescriptionAr, input.Order, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<ZoneParagraph, ZoneParagraphDto>(zoneParagraph);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(ZoneParagraphExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var zoneParagraphs = await _zoneParagraphRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TitleEn, input.TitleAr, input.SubTilteEn, input.SubTitleAr, input.DescriptionEn, input.DescriptionAr, input.OrderMin, input.OrderMax, input.IsActive);
            var items = zoneParagraphs.Select(item => new
            {
                TitleEn = item.ZoneParagraph.TitleEn,
                TitleAr = item.ZoneParagraph.TitleAr,
                SubTilteEn = item.ZoneParagraph.SubTilteEn,
                SubTitleAr = item.ZoneParagraph.SubTitleAr,
                DescriptionEn = item.ZoneParagraph.DescriptionEn,
                DescriptionAr = item.ZoneParagraph.DescriptionAr,
                Order = item.ZoneParagraph.Order,
                IsActive = item.ZoneParagraph.IsActive,

                Zone = item.Zone?.TitleEn,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "ZoneParagraphs.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new ZoneParagraphExcelDownloadTokenCacheItem { Token = token },
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