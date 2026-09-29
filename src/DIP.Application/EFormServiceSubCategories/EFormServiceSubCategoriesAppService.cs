using DIP.Shared;
using DIP.EFormServices;
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
using DIP.EFormServiceSubCategories;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.EFormServiceSubCategories
{

    [Authorize(DIPPermissions.EFormServiceSubCategories.Default)]
    public class EFormServiceSubCategoriesAppService : ApplicationService, IEFormServiceSubCategoriesAppService
    {
        private readonly IDistributedCache<EFormServiceSubCategoryExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IEFormServiceSubCategoryRepository _eFormServiceSubCategoryRepository;
        private readonly EFormServiceSubCategoryManager _eFormServiceSubCategoryManager;
        private readonly IRepository<EFormService, Guid> _eFormServiceRepository;

        public EFormServiceSubCategoriesAppService(IEFormServiceSubCategoryRepository eFormServiceSubCategoryRepository, EFormServiceSubCategoryManager eFormServiceSubCategoryManager, IDistributedCache<EFormServiceSubCategoryExcelDownloadTokenCacheItem, string> excelDownloadTokenCache, IRepository<EFormService, Guid> eFormServiceRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _eFormServiceSubCategoryRepository = eFormServiceSubCategoryRepository;
            _eFormServiceSubCategoryManager = eFormServiceSubCategoryManager; _eFormServiceRepository = eFormServiceRepository;
        }

        public virtual async Task<PagedResultDto<EFormServiceSubCategoryWithNavigationPropertiesDto>> GetListAsync(GetEFormServiceSubCategoriesInput input)
        {
            var totalCount = await _eFormServiceSubCategoryRepository.GetCountAsync(input.FilterText, input.TitleEn, input.TitleAr, input.File, input.OrderMin, input.OrderMax, input.IsActive, input.EFormServiceId);
            var items = await _eFormServiceSubCategoryRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TitleEn, input.TitleAr, input.File, input.OrderMin, input.OrderMax, input.IsActive, input.EFormServiceId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<EFormServiceSubCategoryWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<EFormServiceSubCategoryWithNavigationProperties>, List<EFormServiceSubCategoryWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<EFormServiceSubCategoryWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<EFormServiceSubCategoryWithNavigationProperties, EFormServiceSubCategoryWithNavigationPropertiesDto>
                (await _eFormServiceSubCategoryRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<EFormServiceSubCategoryDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<EFormServiceSubCategory, EFormServiceSubCategoryDto>(await _eFormServiceSubCategoryRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetEFormServiceLookupAsync(LookupRequestDto input)
        {
            var query = (await _eFormServiceRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.TitleEn != null &&
                         x.TitleEn.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<EFormService>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<EFormService>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(DIPPermissions.EFormServiceSubCategories.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _eFormServiceSubCategoryRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.EFormServiceSubCategories.Create)]
        public virtual async Task<EFormServiceSubCategoryDto> CreateAsync(EFormServiceSubCategoryCreateDto input)
        {
            if (input.EFormServiceId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["EFormService"]]);
            }

            var eFormServiceSubCategory = await _eFormServiceSubCategoryManager.CreateAsync(
            input.EFormServiceId, input.TitleEn, input.TitleAr, input.File, input.Order, input.IsActive
            );

            return ObjectMapper.Map<EFormServiceSubCategory, EFormServiceSubCategoryDto>(eFormServiceSubCategory);
        }

        [Authorize(DIPPermissions.EFormServiceSubCategories.Edit)]
        public virtual async Task<EFormServiceSubCategoryDto> UpdateAsync(Guid id, EFormServiceSubCategoryUpdateDto input)
        {
            if (input.EFormServiceId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["EFormService"]]);
            }

            var eFormServiceSubCategory = await _eFormServiceSubCategoryManager.UpdateAsync(
            id,
            input.EFormServiceId, input.TitleEn, input.TitleAr, input.File, input.Order, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<EFormServiceSubCategory, EFormServiceSubCategoryDto>(eFormServiceSubCategory);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(EFormServiceSubCategoryExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var eFormServiceSubCategories = await _eFormServiceSubCategoryRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TitleEn, input.TitleAr, input.File, input.OrderMin, input.OrderMax, input.IsActive);
            var items = eFormServiceSubCategories.Select(item => new
            {
                TitleEn = item.EFormServiceSubCategory.TitleEn,
                TitleAr = item.EFormServiceSubCategory.TitleAr,
                File = item.EFormServiceSubCategory.File,
                Order = item.EFormServiceSubCategory.Order,
                IsActive = item.EFormServiceSubCategory.IsActive,

                EFormService = item.EFormService?.TitleEn,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "EFormServiceSubCategories.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new EFormServiceSubCategoryExcelDownloadTokenCacheItem { Token = token },
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