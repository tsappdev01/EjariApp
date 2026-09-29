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

namespace DIP.LastEventss
{
    public partial class EfCoreLastEventsRepository : EfCoreRepository<DIPDbContext, LastEvents, Guid>, ILastEventsRepository
    {
        public EfCoreLastEventsRepository(IDbContextProvider<DIPDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<List<LastEvents>> GetListAsync(
            string filterText = null,
            string titleAr = null,
            string titleEn = null,
            string metaTitleAr = null,
            string metaTitleEn = null,
            string metaDescriptionEn = null,
            string metaDescriptionAr = null,
            bool? isFeatured = null,
            string slug = null,
            string image = null,
            string headerImage = null,
            string descriptionAr = null,
            string descriptionEn = null,
            string summaryEn = null,
            string summaryAr = null,
            int? orderMin = null,
            int? orderMax = null,
            DateTime? startDateMin = null,
            DateTime? startDateMax = null,
            DateTime? endDateMin = null,
            DateTime? endDateMax = null,
            bool? isActive = null,
            string locationAr = null,
            string locationEn = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, titleAr, titleEn, metaTitleAr, metaTitleEn, metaDescriptionEn, metaDescriptionAr, isFeatured, slug, image, headerImage, descriptionAr, descriptionEn, summaryEn, summaryAr, orderMin, orderMax, startDateMin, startDateMax, endDateMin, endDateMax, isActive, locationAr, locationEn);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? LastEventsConsts.GetDefaultSorting(false) : sorting);
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
            bool? isFeatured = null,
            string slug = null,
            string image = null,
            string headerImage = null,
            string descriptionAr = null,
            string descriptionEn = null,
            string summaryEn = null,
            string summaryAr = null,
            int? orderMin = null,
            int? orderMax = null,
            DateTime? startDateMin = null,
            DateTime? startDateMax = null,
            DateTime? endDateMin = null,
            DateTime? endDateMax = null,
            bool? isActive = null,
            string locationAr = null,
            string locationEn = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, titleAr, titleEn, metaTitleAr, metaTitleEn, metaDescriptionEn, metaDescriptionAr, isFeatured, slug, image, headerImage, descriptionAr, descriptionEn, summaryEn, summaryAr, orderMin, orderMax, startDateMin, startDateMax, endDateMin, endDateMax, isActive, locationAr, locationEn);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<LastEvents> ApplyFilter(
            IQueryable<LastEvents> query,
            string filterText,
            string titleAr = null,
            string titleEn = null,
            string metaTitleAr = null,
            string metaTitleEn = null,
            string metaDescriptionEn = null,
            string metaDescriptionAr = null,
            bool? isFeatured = null,
            string slug = null,
            string image = null,
            string headerImage = null,
            string descriptionAr = null,
            string descriptionEn = null,
            string summaryEn = null,
            string summaryAr = null,
            int? orderMin = null,
            int? orderMax = null,
            DateTime? startDateMin = null,
            DateTime? startDateMax = null,
            DateTime? endDateMin = null,
            DateTime? endDateMax = null,
            bool? isActive = null,
            string locationAr = null,
            string locationEn = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.TitleAr.Contains(filterText) || e.TitleEn.Contains(filterText) || e.MetaTitleAr.Contains(filterText) || e.MetaTitleEn.Contains(filterText) || e.MetaDescriptionEn.Contains(filterText) || e.MetaDescriptionAr.Contains(filterText) || e.Slug.Contains(filterText) || e.Image.Contains(filterText) || e.HeaderImage.Contains(filterText) || e.DescriptionAr.Contains(filterText) || e.DescriptionEn.Contains(filterText) || e.SummaryEn.Contains(filterText) || e.SummaryAr.Contains(filterText) || e.LocationAr.Contains(filterText) || e.LocationEn.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(metaTitleAr), e => e.MetaTitleAr.Contains(metaTitleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(metaTitleEn), e => e.MetaTitleEn.Contains(metaTitleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(metaDescriptionEn), e => e.MetaDescriptionEn.Contains(metaDescriptionEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(metaDescriptionAr), e => e.MetaDescriptionAr.Contains(metaDescriptionAr))
                    .WhereIf(isFeatured.HasValue, e => e.IsFeatured == isFeatured)
                    .WhereIf(!string.IsNullOrWhiteSpace(slug), e => e.Slug.Contains(slug))
                    .WhereIf(!string.IsNullOrWhiteSpace(image), e => e.Image.Contains(image))
                    .WhereIf(!string.IsNullOrWhiteSpace(headerImage), e => e.HeaderImage.Contains(headerImage))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionAr), e => e.DescriptionAr.Contains(descriptionAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionEn), e => e.DescriptionEn.Contains(descriptionEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(summaryEn), e => e.SummaryEn.Contains(summaryEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(summaryAr), e => e.SummaryAr.Contains(summaryAr))
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax.Value)
                    .WhereIf(startDateMin.HasValue, e => e.StartDate >= startDateMin.Value)
                    .WhereIf(startDateMax.HasValue, e => e.StartDate <= startDateMax.Value)
                    .WhereIf(endDateMin.HasValue, e => e.EndDate >= endDateMin.Value)
                    .WhereIf(endDateMax.HasValue, e => e.EndDate <= endDateMax.Value)
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive)
                    .WhereIf(!string.IsNullOrWhiteSpace(locationAr), e => e.LocationAr.Contains(locationAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(locationEn), e => e.LocationEn.Contains(locationEn));
        }
    }
}