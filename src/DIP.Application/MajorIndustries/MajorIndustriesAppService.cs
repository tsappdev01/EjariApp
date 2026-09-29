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
using DIP.MajorIndustries;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.MajorIndustries
{

    [Authorize(DIPPermissions.MajorIndustries.Default)]
    public class MajorIndustriesAppService : ApplicationService, IMajorIndustriesAppService
    {
        private readonly IDistributedCache<MajorIndustryExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IMajorIndustryRepository _majorIndustryRepository;
        private readonly MajorIndustryManager _majorIndustryManager;
        private readonly IRepository<Zone, Guid> _zoneRepository;

        public MajorIndustriesAppService(IMajorIndustryRepository majorIndustryRepository, MajorIndustryManager majorIndustryManager, IDistributedCache<MajorIndustryExcelDownloadTokenCacheItem, string> excelDownloadTokenCache, IRepository<Zone, Guid> zoneRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _majorIndustryRepository = majorIndustryRepository;
            _majorIndustryManager = majorIndustryManager; _zoneRepository = zoneRepository;
        }

        public virtual async Task<PagedResultDto<MajorIndustryWithNavigationPropertiesDto>> GetListAsync(GetMajorIndustriesInput input)
        {
            var totalCount = await _majorIndustryRepository.GetCountAsync(input.FilterText, input.TitleEn, input.TitleAr, input.Image, input.OrderMin, input.OrderMax, input.IsActive, input.ZoneId);
            var items = await _majorIndustryRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TitleEn, input.TitleAr, input.Image, input.OrderMin, input.OrderMax, input.IsActive, input.ZoneId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<MajorIndustryWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<MajorIndustryWithNavigationProperties>, List<MajorIndustryWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<MajorIndustryWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<MajorIndustryWithNavigationProperties, MajorIndustryWithNavigationPropertiesDto>
                (await _majorIndustryRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<MajorIndustryDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<MajorIndustry, MajorIndustryDto>(await _majorIndustryRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetZoneLookupAsync(LookupRequestDto input)
        {
            var query = (await _zoneRepository.GetQueryableAsync())
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

        [Authorize(DIPPermissions.MajorIndustries.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _majorIndustryRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.MajorIndustries.Create)]
        public virtual async Task<MajorIndustryDto> CreateAsync(MajorIndustryCreateDto input)
        {
            if (input.ZoneId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Zone"]]);
            }

            var majorIndustry = await _majorIndustryManager.CreateAsync(
            input.ZoneId, input.TitleEn, input.TitleAr, input.Image, input.Order, input.IsActive
            );

            return ObjectMapper.Map<MajorIndustry, MajorIndustryDto>(majorIndustry);
        }

        [Authorize(DIPPermissions.MajorIndustries.Edit)]
        public virtual async Task<MajorIndustryDto> UpdateAsync(Guid id, MajorIndustryUpdateDto input)
        {
            if (input.ZoneId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Zone"]]);
            }

            var majorIndustry = await _majorIndustryManager.UpdateAsync(
            id,
            input.ZoneId, input.TitleEn, input.TitleAr, input.Image, input.Order, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<MajorIndustry, MajorIndustryDto>(majorIndustry);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(MajorIndustryExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var majorIndustries = await _majorIndustryRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TitleEn, input.TitleAr, input.Image, input.OrderMin, input.OrderMax, input.IsActive);
            var items = majorIndustries.Select(item => new
            {
                TitleEn = item.MajorIndustry.TitleEn,
                TitleAr = item.MajorIndustry.TitleAr,
                Image = item.MajorIndustry.Image,
                Order = item.MajorIndustry.Order,
                IsActive = item.MajorIndustry.IsActive,

                Zone = item.Zone?.TitleEn,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "MajorIndustries.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new MajorIndustryExcelDownloadTokenCacheItem { Token = token },
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