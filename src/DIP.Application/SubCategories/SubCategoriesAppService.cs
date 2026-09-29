using DIP.Shared;
using DIP.Categories;
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
using DIP.SubCategories;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.SubCategories
{

    [Authorize(DIPPermissions.SubCategories.Default)]
    public partial class SubCategoriesAppService : ApplicationService, ISubCategoriesAppService
    {
        private readonly IDistributedCache<SubCategoryExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly SubCategoryManager _subCategoryManager;
        private readonly IRepository<Category, Guid> _categoryRepository;

        public SubCategoriesAppService(ISubCategoryRepository subCategoryRepository, SubCategoryManager subCategoryManager, IDistributedCache<SubCategoryExcelDownloadTokenCacheItem, string> excelDownloadTokenCache, IRepository<Category, Guid> categoryRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _subCategoryRepository = subCategoryRepository;
            _subCategoryManager = subCategoryManager; _categoryRepository = categoryRepository;
        }

        public virtual async Task<PagedResultDto<SubCategoryWithNavigationPropertiesDto>> GetListAsync(GetSubCategoriesInput input)
        {
            var totalCount = await _subCategoryRepository.GetCountAsync(input.FilterText, input.TitleAr, input.TitleEn, input.OrderMin, input.OrderMax, input.IsFeature, input.IsActive, input.CategoryId);
            var items = await _subCategoryRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TitleAr, input.TitleEn, input.OrderMin, input.OrderMax, input.IsFeature, input.IsActive, input.CategoryId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<SubCategoryWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<SubCategoryWithNavigationProperties>, List<SubCategoryWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<SubCategoryWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<SubCategoryWithNavigationProperties, SubCategoryWithNavigationPropertiesDto>
                (await _subCategoryRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<SubCategoryDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<SubCategory, SubCategoryDto>(await _subCategoryRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetCategoryLookupAsync(LookupRequestDto input)
        {
            var query = (await _categoryRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.TitleEn != null &&
                         x.TitleEn.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Category>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Category>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(DIPPermissions.SubCategories.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _subCategoryRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.SubCategories.Create)]
        public virtual async Task<SubCategoryDto> CreateAsync(SubCategoryCreateDto input)
        {
            if (input.CategoryId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Category"]]);
            }

            var subCategory = await _subCategoryManager.CreateAsync(
            input.CategoryId, input.TitleAr, input.TitleEn, input.Order, input.IsFeature, input.IsActive
            );

            return ObjectMapper.Map<SubCategory, SubCategoryDto>(subCategory);
        }

        [Authorize(DIPPermissions.SubCategories.Edit)]
        public virtual async Task<SubCategoryDto> UpdateAsync(Guid id, SubCategoryUpdateDto input)
        {
            if (input.CategoryId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Category"]]);
            }

            var subCategory = await _subCategoryManager.UpdateAsync(
            id,
            input.CategoryId, input.TitleAr, input.TitleEn, input.Order, input.IsFeature, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<SubCategory, SubCategoryDto>(subCategory);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(SubCategoryExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var subCategories = await _subCategoryRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TitleAr, input.TitleEn, input.OrderMin, input.OrderMax, input.IsFeature, input.IsActive);
            var items = subCategories.Select(item => new
            {
                TitleAr = item.SubCategory.TitleAr,
                TitleEn = item.SubCategory.TitleEn,
                Order = item.SubCategory.Order,
                IsFeature = item.SubCategory.IsFeature,
                IsActive = item.SubCategory.IsActive,

                Category = item.Category?.TitleEn,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "SubCategories.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new SubCategoryExcelDownloadTokenCacheItem { Token = token },
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