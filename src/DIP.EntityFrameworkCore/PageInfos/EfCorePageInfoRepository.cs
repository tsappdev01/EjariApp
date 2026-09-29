using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using DIP.EntityFrameworkCore;

namespace DIP.PageInfos
{
    public partial  class EfCorePageInfoRepository : EfCoreRepository<DIPDbContext, PageInfo, Guid>, IPageInfoRepository
    {
        public EfCorePageInfoRepository(IDbContextProvider<DIPDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<List<PageInfo>> GetListAsync(
            string filterText = null,
            string titleAr = null,
            string titleEn = null,
            string metaTitleAr = null,
            string metaTitleEn = null,
            string metaDescriptionEn = null,
            string metaDescriptionAr = null,
            string slug = null,
            string image = null,
            string headerImage = null,
            string youTubeUrl = null,
            string pageInfoArticleTilteEn = null,
            string pageInfoArticleTilteAr = null,
            string pageInfoArticleSubtitleEn = null,
            string pageInfoArticleSubtitleAr = null,
            string descriptionAr = null,
            string descriptionEn = null,
            string summaryEn = null,
            string summaryAr = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, titleAr, titleEn, metaTitleAr, metaTitleEn, metaDescriptionEn, metaDescriptionAr, slug, image, headerImage, youTubeUrl, pageInfoArticleTilteEn, pageInfoArticleTilteAr, pageInfoArticleSubtitleEn, pageInfoArticleSubtitleAr, descriptionAr, descriptionEn, summaryEn, summaryAr, orderMin, orderMax, isActive);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PageInfoConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string titleAr = null,
            string titleEn = null,
            string metaTitleAr = null,
            string metaTitleEn = null,
            string metaDescriptionEn = null,
            string metaDescriptionAr = null,
            string slug = null,
            string image = null,
            string headerImage = null,
            string youTubeUrl = null,
            string pageInfoArticleTilteEn = null,
            string pageInfoArticleTilteAr = null,
            string pageInfoArticleSubtitleEn = null,
            string pageInfoArticleSubtitleAr = null,
            string descriptionAr = null,
            string descriptionEn = null,
            string summaryEn = null,
            string summaryAr = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, titleAr, titleEn, metaTitleAr, metaTitleEn, metaDescriptionEn, metaDescriptionAr, slug, image, headerImage, youTubeUrl, pageInfoArticleTilteEn, pageInfoArticleTilteAr, pageInfoArticleSubtitleEn, pageInfoArticleSubtitleAr, descriptionAr, descriptionEn, summaryEn, summaryAr, orderMin, orderMax, isActive);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<PageInfo> ApplyFilter(
            IQueryable<PageInfo> query,
            string filterText,
            string titleAr = null,
            string titleEn = null,
            string metaTitleAr = null,
            string metaTitleEn = null,
            string metaDescriptionEn = null,
            string metaDescriptionAr = null,
            string slug = null,
            string image = null,
            string headerImage = null,
            string youTubeUrl = null,
            string pageInfoArticleTilteEn = null,
            string pageInfoArticleTilteAr = null,
            string pageInfoArticleSubtitleEn = null,
            string pageInfoArticleSubtitleAr = null,
            string descriptionAr = null,
            string descriptionEn = null,
            string summaryEn = null,
            string summaryAr = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.TitleAr.Contains(filterText) || e.TitleEn.Contains(filterText) || e.MetaTitleAr.Contains(filterText) || e.MetaTitleEn.Contains(filterText) || e.MetaDescriptionEn.Contains(filterText) || e.MetaDescriptionAr.Contains(filterText) || e.Slug.Contains(filterText) || e.Image.Contains(filterText) || e.HeaderImage.Contains(filterText) || e.YouTubeUrl.Contains(filterText) || e.PageInfoArticleTilteEn.Contains(filterText) || e.PageInfoArticleTilteAr.Contains(filterText) || e.PageInfoArticleSubtitleEn.Contains(filterText) || e.PageInfoArticleSubtitleAr.Contains(filterText) || e.DescriptionAr.Contains(filterText) || e.DescriptionEn.Contains(filterText) || e.SummaryEn.Contains(filterText) || e.SummaryAr.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(metaTitleAr), e => e.MetaTitleAr.Contains(metaTitleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(metaTitleEn), e => e.MetaTitleEn.Contains(metaTitleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(metaDescriptionEn), e => e.MetaDescriptionEn.Contains(metaDescriptionEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(metaDescriptionAr), e => e.MetaDescriptionAr.Contains(metaDescriptionAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(slug), e => e.Slug.Contains(slug))
                    .WhereIf(!string.IsNullOrWhiteSpace(image), e => e.Image.Contains(image))
                    .WhereIf(!string.IsNullOrWhiteSpace(headerImage), e => e.HeaderImage.Contains(headerImage))
                    .WhereIf(!string.IsNullOrWhiteSpace(youTubeUrl), e => e.YouTubeUrl.Contains(youTubeUrl))
                    .WhereIf(!string.IsNullOrWhiteSpace(pageInfoArticleTilteEn), e => e.PageInfoArticleTilteEn.Contains(pageInfoArticleTilteEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(pageInfoArticleTilteAr), e => e.PageInfoArticleTilteAr.Contains(pageInfoArticleTilteAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(pageInfoArticleSubtitleEn), e => e.PageInfoArticleSubtitleEn.Contains(pageInfoArticleSubtitleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(pageInfoArticleSubtitleAr), e => e.PageInfoArticleSubtitleAr.Contains(pageInfoArticleSubtitleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionAr), e => e.DescriptionAr.Contains(descriptionAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionEn), e => e.DescriptionEn.Contains(descriptionEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(summaryEn), e => e.SummaryEn.Contains(summaryEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(summaryAr), e => e.SummaryAr.Contains(summaryAr))
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax.Value)
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive);
        }
    }
}