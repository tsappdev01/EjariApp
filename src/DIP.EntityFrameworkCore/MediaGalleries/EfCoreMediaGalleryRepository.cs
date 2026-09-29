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

namespace DIP.MediaGalleries
{
    public partial class EfCoreMediaGalleryRepository : EfCoreRepository<DIPDbContext, MediaGallery, Guid>, IMediaGalleryRepository
    {
        public EfCoreMediaGalleryRepository(IDbContextProvider<DIPDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<List<MediaGallery>> GetListAsync(
            string filterText = null,
            string titleEn = null,
            string slug = null,
            string titleAr = null,
            string metaTitleEn = null,
            string metaTitleAr = null,
            string metaDescriptionEn = null,
            string metaDescriptionAr = null,
            string summaryEn = null,
            string summaryAr = null,
            string headerImage = null,
            string image = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, titleEn, slug, titleAr, metaTitleEn, metaTitleAr, metaDescriptionEn, metaDescriptionAr, summaryEn, summaryAr, headerImage, image, orderMin, orderMax, isActive);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? MediaGalleryConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string titleEn = null,
            string slug = null,
            string titleAr = null,
            string metaTitleEn = null,
            string metaTitleAr = null,
            string metaDescriptionEn = null,
            string metaDescriptionAr = null,
            string summaryEn = null,
            string summaryAr = null,
            string headerImage = null,
            string image = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, titleEn, slug, titleAr, metaTitleEn, metaTitleAr, metaDescriptionEn, metaDescriptionAr, summaryEn, summaryAr, headerImage, image, orderMin, orderMax, isActive);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<MediaGallery> ApplyFilter(
            IQueryable<MediaGallery> query,
            string filterText,
            string titleEn = null,
            string slug = null,
            string titleAr = null,
            string metaTitleEn = null,
            string metaTitleAr = null,
            string metaDescriptionEn = null,
            string metaDescriptionAr = null,
            string summaryEn = null,
            string summaryAr = null,
            string headerImage = null,
            string image = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.TitleEn.Contains(filterText) || e.Slug.Contains(filterText) || e.TitleAr.Contains(filterText) || e.MetaTitleEn.Contains(filterText) || e.MetaTitleAr.Contains(filterText) || e.MetaDescriptionEn.Contains(filterText) || e.MetaDescriptionAr.Contains(filterText) || e.SummaryEn.Contains(filterText) || e.SummaryAr.Contains(filterText) || e.HeaderImage.Contains(filterText) || e.Image.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(slug), e => e.Slug.Contains(slug))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(metaTitleEn), e => e.MetaTitleEn.Contains(metaTitleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(metaTitleAr), e => e.MetaTitleAr.Contains(metaTitleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(metaDescriptionEn), e => e.MetaDescriptionEn.Contains(metaDescriptionEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(metaDescriptionAr), e => e.MetaDescriptionAr.Contains(metaDescriptionAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(summaryEn), e => e.SummaryEn.Contains(summaryEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(summaryAr), e => e.SummaryAr.Contains(summaryAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(headerImage), e => e.HeaderImage.Contains(headerImage))
                    .WhereIf(!string.IsNullOrWhiteSpace(image), e => e.Image.Contains(image))
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax.Value)
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive);
        }
    }
}