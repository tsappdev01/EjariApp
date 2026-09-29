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

namespace DIP.Zones
{
    public partial class EfCoreZoneRepository : EfCoreRepository<DIPDbContext, Zone, Guid>, IZoneRepository
    {
        public EfCoreZoneRepository(IDbContextProvider<DIPDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<List<Zone>> GetListAsync(
            string filterText = null,
            string titleEn = null,
            string titleAR = null,
            string metaTitleEn = null,
            string metaDescriptionEn = null,
            string metaTitleAr = null,
            string metaDescriptionAr = null,
            string slug = null,
            string summaryEn = null,
            string summaryAr = null,
            string image = null,
            string headerImage = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isFeature = null,
            bool? isActive = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, titleEn, titleAR, metaTitleEn, metaDescriptionEn, metaTitleAr, metaDescriptionAr, slug, summaryEn, summaryAr, image, headerImage, orderMin, orderMax, isFeature, isActive);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ZoneConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string titleEn = null,
            string titleAR = null,
            string metaTitleEn = null,
            string metaDescriptionEn = null,
            string metaTitleAr = null,
            string metaDescriptionAr = null,
            string slug = null,
            string summaryEn = null,
            string summaryAr = null,
            string image = null,
            string headerImage = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isFeature = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, titleEn, titleAR, metaTitleEn, metaDescriptionEn, metaTitleAr, metaDescriptionAr, slug, summaryEn, summaryAr, image, headerImage, orderMin, orderMax, isFeature, isActive);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Zone> ApplyFilter(
            IQueryable<Zone> query,
            string filterText,
            string titleEn = null,
            string titleAR = null,
            string metaTitleEn = null,
            string metaDescriptionEn = null,
            string metaTitleAr = null,
            string metaDescriptionAr = null,
            string slug = null,
            string summaryEn = null,
            string summaryAr = null,
            string image = null,
            string headerImage = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isFeature = null,
            bool? isActive = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.TitleEn.Contains(filterText) || e.TitleAR.Contains(filterText) || e.MetaTitleEn.Contains(filterText) || e.MetaDescriptionEn.Contains(filterText) || e.MetaTitleAr.Contains(filterText) || e.MetaDescriptionAr.Contains(filterText) || e.Slug.Contains(filterText) || e.SummaryEn.Contains(filterText) || e.SummaryAr.Contains(filterText) || e.Image.Contains(filterText) || e.HeaderImage.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAR), e => e.TitleAR.Contains(titleAR))
                    .WhereIf(!string.IsNullOrWhiteSpace(metaTitleEn), e => e.MetaTitleEn.Contains(metaTitleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(metaDescriptionEn), e => e.MetaDescriptionEn.Contains(metaDescriptionEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(metaTitleAr), e => e.MetaTitleAr.Contains(metaTitleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(metaDescriptionAr), e => e.MetaDescriptionAr.Contains(metaDescriptionAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(slug), e => e.Slug.Contains(slug))
                    .WhereIf(!string.IsNullOrWhiteSpace(summaryEn), e => e.SummaryEn.Contains(summaryEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(summaryAr), e => e.SummaryAr.Contains(summaryAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(image), e => e.Image.Contains(image))
                    .WhereIf(!string.IsNullOrWhiteSpace(headerImage), e => e.HeaderImage.Contains(headerImage))
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax.Value)
                    .WhereIf(isFeature.HasValue, e => e.IsFeature == isFeature)
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive);
        }
    }
}