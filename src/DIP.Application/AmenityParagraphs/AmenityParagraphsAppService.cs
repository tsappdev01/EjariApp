using DIP.Shared;
using DIP.Amenities;
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
using DIP.AmenityParagraphs;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.AmenityParagraphs
{

    [Authorize(DIPPermissions.AmenityParagraphs.Default)]
    public class AmenityParagraphsAppService : ApplicationService, IAmenityParagraphsAppService
    {
        private readonly IDistributedCache<AmenityParagraphExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IAmenityParagraphRepository _amenityParagraphRepository;
        private readonly AmenityParagraphManager _amenityParagraphManager;
        private readonly IRepository<Amenity, Guid> _amenityRepository;

        public AmenityParagraphsAppService(IAmenityParagraphRepository amenityParagraphRepository, AmenityParagraphManager amenityParagraphManager, IDistributedCache<AmenityParagraphExcelDownloadTokenCacheItem, string> excelDownloadTokenCache, IRepository<Amenity, Guid> amenityRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _amenityParagraphRepository = amenityParagraphRepository;
            _amenityParagraphManager = amenityParagraphManager; _amenityRepository = amenityRepository;
        }

        public virtual async Task<PagedResultDto<AmenityParagraphWithNavigationPropertiesDto>> GetListAsync(GetAmenityParagraphsInput input)
        {
            var totalCount = await _amenityParagraphRepository.GetCountAsync(input.FilterText, input.TitleEn, input.TitleAr, input.SubTitleEn, input.SubTitleAr, input.DescriptionEn, input.DescriptionAr, input.ButtonUrl, input.OrderMin, input.OrderMax, input.IsActive, input.AmenityId);
            var items = await _amenityParagraphRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TitleEn, input.TitleAr, input.SubTitleEn, input.SubTitleAr, input.DescriptionEn, input.DescriptionAr, input.ButtonUrl, input.OrderMin, input.OrderMax, input.IsActive, input.AmenityId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<AmenityParagraphWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<AmenityParagraphWithNavigationProperties>, List<AmenityParagraphWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<AmenityParagraphWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<AmenityParagraphWithNavigationProperties, AmenityParagraphWithNavigationPropertiesDto>
                (await _amenityParagraphRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<AmenityParagraphDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<AmenityParagraph, AmenityParagraphDto>(await _amenityParagraphRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetAmenityLookupAsync(LookupRequestDto input)
        {
            var query = (await _amenityRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.TitleEn != null &&
                         x.TitleEn.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Amenity>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Amenity>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(DIPPermissions.AmenityParagraphs.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _amenityParagraphRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.AmenityParagraphs.Create)]
        public virtual async Task<AmenityParagraphDto> CreateAsync(AmenityParagraphCreateDto input)
        {
            if (input.AmenityId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Amenity"]]);
            }

            var amenityParagraph = await _amenityParagraphManager.CreateAsync(
            input.AmenityId, input.TitleEn, input.TitleAr, input.SubTitleEn, input.SubTitleAr, input.DescriptionEn, input.DescriptionAr, input.ButtonUrl, input.Order, input.IsActive
            );

            return ObjectMapper.Map<AmenityParagraph, AmenityParagraphDto>(amenityParagraph);
        }

        [Authorize(DIPPermissions.AmenityParagraphs.Edit)]
        public virtual async Task<AmenityParagraphDto> UpdateAsync(Guid id, AmenityParagraphUpdateDto input)
        {
            if (input.AmenityId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Amenity"]]);
            }

            var amenityParagraph = await _amenityParagraphManager.UpdateAsync(
            id,
            input.AmenityId, input.TitleEn, input.TitleAr, input.SubTitleEn, input.SubTitleAr, input.DescriptionEn, input.DescriptionAr, input.ButtonUrl, input.Order, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<AmenityParagraph, AmenityParagraphDto>(amenityParagraph);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(AmenityParagraphExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var amenityParagraphs = await _amenityParagraphRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TitleEn, input.TitleAr, input.SubTitleEn, input.SubTitleAr, input.DescriptionEn, input.DescriptionAr, input.ButtonUrl, input.OrderMin, input.OrderMax, input.IsActive);
            var items = amenityParagraphs.Select(item => new
            {
                TitleEn = item.AmenityParagraph.TitleEn,
                TitleAr = item.AmenityParagraph.TitleAr,
                SubTitleEn = item.AmenityParagraph.SubTitleEn,
                SubTitleAr = item.AmenityParagraph.SubTitleAr,
                DescriptionEn = item.AmenityParagraph.DescriptionEn,
                DescriptionAr = item.AmenityParagraph.DescriptionAr,
                ButtonUrl = item.AmenityParagraph.ButtonUrl,
                Order = item.AmenityParagraph.Order,
                IsActive = item.AmenityParagraph.IsActive,

                Amenity = item.Amenity?.TitleEn,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "AmenityParagraphs.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new AmenityParagraphExcelDownloadTokenCacheItem { Token = token },
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