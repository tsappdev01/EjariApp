using DIP.Shared;
using DIP.SubCategories;
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
using DIP.Commercials;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.Commercials
{

    [Authorize(DIPPermissions.Commercials.Default)]
    public partial class CommercialsAppService : ApplicationService, ICommercialsAppService
    {
        private readonly IDistributedCache<CommercialExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly ICommercialRepository _commercialRepository;
        private readonly CommercialManager _commercialManager;
        private readonly IRepository<SubCategory, Guid> _subCategoryRepository;

        public CommercialsAppService(ICommercialRepository commercialRepository, CommercialManager commercialManager, IDistributedCache<CommercialExcelDownloadTokenCacheItem, string> excelDownloadTokenCache, IRepository<SubCategory, Guid> subCategoryRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _commercialRepository = commercialRepository;
            _commercialManager = commercialManager; _subCategoryRepository = subCategoryRepository;
        }

        public virtual async Task<PagedResultDto<CommercialWithNavigationPropertiesDto>> GetListAsync(GetCommercialsInput input)
        {
            var totalCount = await _commercialRepository.GetCountAsync(input.FilterText, input.TitleEn, input.TitleAr, input.PlotNo, input.ActivityEn, input.ActivityAr, input.Phone, input.Fax, input.MakaniNo, input.IsActive, input.OrderMin, input.OrderMax, input.SubCategoryId);
            var items = await _commercialRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TitleEn, input.TitleAr, input.PlotNo, input.ActivityEn, input.ActivityAr, input.Phone, input.Fax, input.MakaniNo, input.IsActive, input.OrderMin, input.OrderMax, input.SubCategoryId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<CommercialWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<CommercialWithNavigationProperties>, List<CommercialWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<CommercialWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<CommercialWithNavigationProperties, CommercialWithNavigationPropertiesDto>
                (await _commercialRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<CommercialDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Commercial, CommercialDto>(await _commercialRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetSubCategoryLookupAsync(LookupRequestDto input)
        {
            var query = (await _subCategoryRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.TitleEn != null &&
                         x.TitleEn.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<SubCategory>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<SubCategory>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(DIPPermissions.Commercials.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _commercialRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.Commercials.Create)]
        public virtual async Task<CommercialDto> CreateAsync(CommercialCreateDto input)
        {
            if (input.SubCategoryId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["SubCategory"]]);
            }

            var commercial = await _commercialManager.CreateAsync(
            input.SubCategoryId, input.TitleEn, input.TitleAr, input.PlotNo, input.ActivityEn, input.ActivityAr, input.Phone, input.Fax, input.MakaniNo, input.IsActive, input.Order
            );

            return ObjectMapper.Map<Commercial, CommercialDto>(commercial);
        }

        [Authorize(DIPPermissions.Commercials.Edit)]
        public virtual async Task<CommercialDto> UpdateAsync(Guid id, CommercialUpdateDto input)
        {
            if (input.SubCategoryId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["SubCategory"]]);
            }

            var commercial = await _commercialManager.UpdateAsync(
            id,
            input.SubCategoryId, input.TitleEn, input.TitleAr, input.PlotNo, input.ActivityEn, input.ActivityAr, input.Phone, input.Fax, input.MakaniNo, input.IsActive, input.Order, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Commercial, CommercialDto>(commercial);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(CommercialExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var commercials = await _commercialRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TitleEn, input.TitleAr, input.PlotNo, input.ActivityEn, input.ActivityAr, input.Phone, input.Fax, input.MakaniNo, input.IsActive, input.OrderMin, input.OrderMax);
            var items = commercials.Select(item => new
            {
                TitleEn = item.Commercial.TitleEn,
                TitleAr = item.Commercial.TitleAr,
                PlotNo = item.Commercial.PlotNo,
                ActivityEn = item.Commercial.ActivityEn,
                ActivityAr = item.Commercial.ActivityAr,
                Phone = item.Commercial.Phone,
                Fax = item.Commercial.Fax,
                MakaniNo = item.Commercial.MakaniNo,
                IsActive = item.Commercial.IsActive,
                Order = item.Commercial.Order,

                SubCategory = item.SubCategory?.TitleEn,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Commercials.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new CommercialExcelDownloadTokenCacheItem { Token = token },
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