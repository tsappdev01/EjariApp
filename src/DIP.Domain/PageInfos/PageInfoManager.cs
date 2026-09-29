using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.PageInfos
{
    public class PageInfoManager : DomainService
    {
        private readonly IPageInfoRepository _pageInfoRepository;

        public PageInfoManager(IPageInfoRepository pageInfoRepository)
        {
            _pageInfoRepository = pageInfoRepository;
        }

        public async Task<PageInfo> CreateAsync(
        string titleAr, string titleEn, string metaTitleAr, string metaTitleEn, string metaDescriptionEn, string metaDescriptionAr, string slug, string image, string headerImage, string youTubeUrl, string pageInfoArticleTilteEn, string pageInfoArticleTilteAr, string pageInfoArticleSubtitleEn, string pageInfoArticleSubtitleAr, string descriptionAr, string descriptionEn, string summaryEn, string summaryAr, int order, bool isActive)
        {
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(slug, nameof(slug));

            var pageInfo = new PageInfo(
             GuidGenerator.Create(),
             titleAr, titleEn, metaTitleAr, metaTitleEn, metaDescriptionEn, metaDescriptionAr, slug, image, headerImage, youTubeUrl, pageInfoArticleTilteEn, pageInfoArticleTilteAr, pageInfoArticleSubtitleEn, pageInfoArticleSubtitleAr, descriptionAr, descriptionEn, summaryEn, summaryAr, order, isActive
             );

            return await _pageInfoRepository.InsertAsync(pageInfo);
        }

        public async Task<PageInfo> UpdateAsync(
            Guid id,
            string titleAr, string titleEn, string metaTitleAr, string metaTitleEn, string metaDescriptionEn, string metaDescriptionAr, string slug, string image, string headerImage, string youTubeUrl, string pageInfoArticleTilteEn, string pageInfoArticleTilteAr, string pageInfoArticleSubtitleEn, string pageInfoArticleSubtitleAr, string descriptionAr, string descriptionEn, string summaryEn, string summaryAr, int order, bool isActive, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(slug, nameof(slug));

            var pageInfo = await _pageInfoRepository.GetAsync(id);

            pageInfo.TitleAr = titleAr;
            pageInfo.TitleEn = titleEn;
            pageInfo.MetaTitleAr = metaTitleAr;
            pageInfo.MetaTitleEn = metaTitleEn;
            pageInfo.MetaDescriptionEn = metaDescriptionEn;
            pageInfo.MetaDescriptionAr = metaDescriptionAr;
            pageInfo.Slug = slug;
            pageInfo.Image = image;
            pageInfo.HeaderImage = headerImage;
            pageInfo.YouTubeUrl = youTubeUrl;
            pageInfo.PageInfoArticleTilteEn = pageInfoArticleTilteEn;
            pageInfo.PageInfoArticleTilteAr = pageInfoArticleTilteAr;
            pageInfo.PageInfoArticleSubtitleEn = pageInfoArticleSubtitleEn;
            pageInfo.PageInfoArticleSubtitleAr = pageInfoArticleSubtitleAr;
            pageInfo.DescriptionAr = descriptionAr;
            pageInfo.DescriptionEn = descriptionEn;
            pageInfo.SummaryEn = summaryEn;
            pageInfo.SummaryAr = summaryAr;
            pageInfo.Order = order;
            pageInfo.IsActive = isActive;

            pageInfo.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _pageInfoRepository.UpdateAsync(pageInfo);
        }

    }
}