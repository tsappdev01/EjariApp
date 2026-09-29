using DIP.Amenities;
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

namespace DIP.AmenityParagraphs
{
    public class EfCoreAmenityParagraphRepository : EfCoreRepository<DIPDbContext, AmenityParagraph, Guid>, IAmenityParagraphRepository
    {
        public EfCoreAmenityParagraphRepository(IDbContextProvider<DIPDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<AmenityParagraphWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(amenityParagraph => new AmenityParagraphWithNavigationProperties
                {
                    AmenityParagraph = amenityParagraph,
                    Amenity = dbContext.Set<Amenity>().FirstOrDefault(c => c.Id == amenityParagraph.AmenityId)
                }).FirstOrDefault();
        }

        public async Task<List<AmenityParagraphWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string subTitleEn = null,
            string subTitleAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string buttonUrl = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? amenityId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, titleEn, titleAr, subTitleEn, subTitleAr, descriptionEn, descriptionAr, buttonUrl, orderMin, orderMax, isActive, amenityId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? AmenityParagraphConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<AmenityParagraphWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from amenityParagraph in (await GetDbSetAsync())
                   join amenity in (await GetDbContextAsync()).Set<Amenity>() on amenityParagraph.AmenityId equals amenity.Id into amenities
                   from amenity in amenities.DefaultIfEmpty()
                   select new AmenityParagraphWithNavigationProperties
                   {
                       AmenityParagraph = amenityParagraph,
                       Amenity = amenity
                   };
        }

        protected virtual IQueryable<AmenityParagraphWithNavigationProperties> ApplyFilter(
            IQueryable<AmenityParagraphWithNavigationProperties> query,
            string filterText,
            string titleEn = null,
            string titleAr = null,
            string subTitleEn = null,
            string subTitleAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string buttonUrl = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? amenityId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.AmenityParagraph.TitleEn.Contains(filterText) || e.AmenityParagraph.TitleAr.Contains(filterText) || e.AmenityParagraph.SubTitleEn.Contains(filterText) || e.AmenityParagraph.SubTitleAr.Contains(filterText) || e.AmenityParagraph.DescriptionEn.Contains(filterText) || e.AmenityParagraph.DescriptionAr.Contains(filterText) || e.AmenityParagraph.ButtonUrl.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.AmenityParagraph.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.AmenityParagraph.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(subTitleEn), e => e.AmenityParagraph.SubTitleEn.Contains(subTitleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(subTitleAr), e => e.AmenityParagraph.SubTitleAr.Contains(subTitleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionEn), e => e.AmenityParagraph.DescriptionEn.Contains(descriptionEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionAr), e => e.AmenityParagraph.DescriptionAr.Contains(descriptionAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(buttonUrl), e => e.AmenityParagraph.ButtonUrl.Contains(buttonUrl))
                    .WhereIf(orderMin.HasValue, e => e.AmenityParagraph.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.AmenityParagraph.Order <= orderMax.Value)
                    .WhereIf(isActive.HasValue, e => e.AmenityParagraph.IsActive == isActive)
                    .WhereIf(amenityId != null && amenityId != Guid.Empty, e => e.Amenity != null && e.Amenity.Id == amenityId);
        }

        public async Task<List<AmenityParagraph>> GetListAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string subTitleEn = null,
            string subTitleAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string buttonUrl = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, titleEn, titleAr, subTitleEn, subTitleAr, descriptionEn, descriptionAr, buttonUrl, orderMin, orderMax, isActive);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? AmenityParagraphConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string subTitleEn = null,
            string subTitleAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string buttonUrl = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? amenityId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, titleEn, titleAr, subTitleEn, subTitleAr, descriptionEn, descriptionAr, buttonUrl, orderMin, orderMax, isActive, amenityId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<AmenityParagraph> ApplyFilter(
            IQueryable<AmenityParagraph> query,
            string filterText,
            string titleEn = null,
            string titleAr = null,
            string subTitleEn = null,
            string subTitleAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string buttonUrl = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.TitleEn.Contains(filterText) || e.TitleAr.Contains(filterText) || e.SubTitleEn.Contains(filterText) || e.SubTitleAr.Contains(filterText) || e.DescriptionEn.Contains(filterText) || e.DescriptionAr.Contains(filterText) || e.ButtonUrl.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(subTitleEn), e => e.SubTitleEn.Contains(subTitleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(subTitleAr), e => e.SubTitleAr.Contains(subTitleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionEn), e => e.DescriptionEn.Contains(descriptionEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionAr), e => e.DescriptionAr.Contains(descriptionAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(buttonUrl), e => e.ButtonUrl.Contains(buttonUrl))
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax.Value)
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive);
        }
    }
}