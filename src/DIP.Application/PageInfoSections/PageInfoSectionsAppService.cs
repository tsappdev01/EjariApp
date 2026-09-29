using DIP.Shared;
using DIP.PageInfos;
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
using DIP.PageInfoSections;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.PageInfoSections
{

    [Authorize(DIPPermissions.PageInfoSections.Default)]
    public class PageInfoSectionsAppService : ApplicationService, IPageInfoSectionsAppService
    {
        private readonly IDistributedCache<PageInfoSectionExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IPageInfoSectionRepository _pageInfoSectionRepository;
        private readonly PageInfoSectionManager _pageInfoSectionManager;
        private readonly IRepository<PageInfo, Guid> _pageInfoRepository;

        public PageInfoSectionsAppService(IPageInfoSectionRepository pageInfoSectionRepository, PageInfoSectionManager pageInfoSectionManager, IDistributedCache<PageInfoSectionExcelDownloadTokenCacheItem, string> excelDownloadTokenCache, IRepository<PageInfo, Guid> pageInfoRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _pageInfoSectionRepository = pageInfoSectionRepository;
            _pageInfoSectionManager = pageInfoSectionManager; _pageInfoRepository = pageInfoRepository;
        }

        public virtual async Task<PagedResultDto<PageInfoSectionWithNavigationPropertiesDto>> GetListAsync(GetPageInfoSectionsInput input)
        {
            var totalCount = await _pageInfoSectionRepository.GetCountAsync(input.FilterText, input.TitleEn, input.TitleAr, input.SubTitleEn, input.SubTitleAr, input.SummaryEn, input.SummaryAr, input.DescriptionEn, input.DescriptionAr, input.PageSectionMedia, input.YoutubeUrl, input.OrderMin, input.OrderMax, input.IsActive, input.PageInfoId);
            var items = await _pageInfoSectionRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TitleEn, input.TitleAr, input.SubTitleEn, input.SubTitleAr, input.SummaryEn, input.SummaryAr, input.DescriptionEn, input.DescriptionAr, input.PageSectionMedia, input.YoutubeUrl, input.OrderMin, input.OrderMax, input.IsActive, input.PageInfoId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<PageInfoSectionWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<PageInfoSectionWithNavigationProperties>, List<PageInfoSectionWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<PageInfoSectionWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<PageInfoSectionWithNavigationProperties, PageInfoSectionWithNavigationPropertiesDto>
                (await _pageInfoSectionRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<PageInfoSectionDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<PageInfoSection, PageInfoSectionDto>(await _pageInfoSectionRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetPageInfoLookupAsync(LookupRequestDto input)
        {
            var query = (await _pageInfoRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.TitleEn != null &&
                         x.TitleEn.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<PageInfo>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<PageInfo>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(DIPPermissions.PageInfoSections.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _pageInfoSectionRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.PageInfoSections.Create)]
        public virtual async Task<PageInfoSectionDto> CreateAsync(PageInfoSectionCreateDto input)
        {
            if (input.PageInfoId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["PageInfo"]]);
            }

            var pageInfoSection = await _pageInfoSectionManager.CreateAsync(
            input.PageInfoId, input.TitleEn, input.TitleAr, input.SubTitleEn, input.SubTitleAr, input.SummaryEn, input.SummaryAr, input.DescriptionEn, input.DescriptionAr, input.PageSectionMedia, input.YoutubeUrl, input.Order, input.IsActive
            );

            return ObjectMapper.Map<PageInfoSection, PageInfoSectionDto>(pageInfoSection);
        }

        [Authorize(DIPPermissions.PageInfoSections.Edit)]
        public virtual async Task<PageInfoSectionDto> UpdateAsync(Guid id, PageInfoSectionUpdateDto input)
        {
            if (input.PageInfoId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["PageInfo"]]);
            }

            var pageInfoSection = await _pageInfoSectionManager.UpdateAsync(
            id,
            input.PageInfoId, input.TitleEn, input.TitleAr, input.SubTitleEn, input.SubTitleAr, input.SummaryEn, input.SummaryAr, input.DescriptionEn, input.DescriptionAr, input.PageSectionMedia, input.YoutubeUrl, input.Order, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<PageInfoSection, PageInfoSectionDto>(pageInfoSection);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(PageInfoSectionExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var pageInfoSections = await _pageInfoSectionRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TitleEn, input.TitleAr, input.SubTitleEn, input.SubTitleAr, input.SummaryEn, input.SummaryAr, input.DescriptionEn, input.DescriptionAr, input.PageSectionMedia, input.YoutubeUrl, input.OrderMin, input.OrderMax, input.IsActive);
            var items = pageInfoSections.Select(item => new
            {
                TitleEn = item.PageInfoSection.TitleEn,
                TitleAr = item.PageInfoSection.TitleAr,
                SubTitleEn = item.PageInfoSection.SubTitleEn,
                SubTitleAr = item.PageInfoSection.SubTitleAr,
                SummaryEn = item.PageInfoSection.SummaryEn,
                SummaryAr = item.PageInfoSection.SummaryAr,
                DescriptionEn = item.PageInfoSection.DescriptionEn,
                DescriptionAr = item.PageInfoSection.DescriptionAr,
                PageSectionMedia = item.PageInfoSection.PageSectionMedia,
                YoutubeUrl = item.PageInfoSection.YoutubeUrl,
                Order = item.PageInfoSection.Order,
                IsActive = item.PageInfoSection.IsActive,

                PageInfo = item.PageInfo?.TitleEn,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "PageInfoSections.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new PageInfoSectionExcelDownloadTokenCacheItem { Token = token },
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