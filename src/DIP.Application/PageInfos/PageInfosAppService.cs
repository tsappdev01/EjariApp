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
using DIP.PageInfos;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.PageInfos
{

    [Authorize(DIPPermissions.PageInfos.Default)]
    public partial class PageInfosAppService : ApplicationService, IPageInfosAppService
    {
        private readonly IDistributedCache<PageInfoExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IPageInfoRepository _pageInfoRepository;
        private readonly PageInfoManager _pageInfoManager;

        public PageInfosAppService(IPageInfoRepository pageInfoRepository, PageInfoManager pageInfoManager, IDistributedCache<PageInfoExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _pageInfoRepository = pageInfoRepository;
            _pageInfoManager = pageInfoManager;
        }

        public virtual async Task<PagedResultDto<PageInfoDto>> GetListAsync(GetPageInfosInput input)
        {
            var totalCount = await _pageInfoRepository.GetCountAsync(input.FilterText, input.TitleAr, input.TitleEn, input.MetaTitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaDescriptionAr, input.Slug, input.Image, input.HeaderImage, input.YouTubeUrl, input.PageInfoArticleTilteEn, input.PageInfoArticleTilteAr, input.PageInfoArticleSubtitleEn, input.PageInfoArticleSubtitleAr, input.DescriptionAr, input.DescriptionEn, input.SummaryEn, input.SummaryAr, input.OrderMin, input.OrderMax, input.IsActive);
            var items = await _pageInfoRepository.GetListAsync(input.FilterText, input.TitleAr, input.TitleEn, input.MetaTitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaDescriptionAr, input.Slug, input.Image, input.HeaderImage, input.YouTubeUrl, input.PageInfoArticleTilteEn, input.PageInfoArticleTilteAr, input.PageInfoArticleSubtitleEn, input.PageInfoArticleSubtitleAr, input.DescriptionAr, input.DescriptionEn, input.SummaryEn, input.SummaryAr, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<PageInfoDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<PageInfo>, List<PageInfoDto>>(items)
            };
        }

        public virtual async Task<PageInfoDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<PageInfo, PageInfoDto>(await _pageInfoRepository.GetAsync(id));
        }

        [Authorize(DIPPermissions.PageInfos.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _pageInfoRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.PageInfos.Create)]
        public virtual async Task<PageInfoDto> CreateAsync(PageInfoCreateDto input)
        {

            var pageInfo = await _pageInfoManager.CreateAsync(
            input.TitleAr, input.TitleEn, input.MetaTitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaDescriptionAr, input.Slug, input.Image, input.HeaderImage, input.YouTubeUrl, input.PageInfoArticleTilteEn, input.PageInfoArticleTilteAr, input.PageInfoArticleSubtitleEn, input.PageInfoArticleSubtitleAr, input.DescriptionAr, input.DescriptionEn, input.SummaryEn, input.SummaryAr, input.Order, input.IsActive
            );

            return ObjectMapper.Map<PageInfo, PageInfoDto>(pageInfo);
        }

        [Authorize(DIPPermissions.PageInfos.Edit)]
        public virtual async Task<PageInfoDto> UpdateAsync(Guid id, PageInfoUpdateDto input)
        {

            var pageInfo = await _pageInfoManager.UpdateAsync(
            id,
            input.TitleAr, input.TitleEn, input.MetaTitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaDescriptionAr, input.Slug, input.Image, input.HeaderImage, input.YouTubeUrl, input.PageInfoArticleTilteEn, input.PageInfoArticleTilteAr, input.PageInfoArticleSubtitleEn, input.PageInfoArticleSubtitleAr, input.DescriptionAr, input.DescriptionEn, input.SummaryEn, input.SummaryAr, input.Order, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<PageInfo, PageInfoDto>(pageInfo);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(PageInfoExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _pageInfoRepository.GetListAsync(input.FilterText, input.TitleAr, input.TitleEn, input.MetaTitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaDescriptionAr, input.Slug, input.Image, input.HeaderImage, input.YouTubeUrl, input.PageInfoArticleTilteEn, input.PageInfoArticleTilteAr, input.PageInfoArticleSubtitleEn, input.PageInfoArticleSubtitleAr, input.DescriptionAr, input.DescriptionEn, input.SummaryEn, input.SummaryAr, input.OrderMin, input.OrderMax, input.IsActive);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<PageInfo>, List<PageInfoExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "PageInfos.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new PageInfoExcelDownloadTokenCacheItem { Token = token },
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