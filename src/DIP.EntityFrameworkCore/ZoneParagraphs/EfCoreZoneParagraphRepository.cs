using DIP.Zones;
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

namespace DIP.ZoneParagraphs
{
    public class EfCoreZoneParagraphRepository : EfCoreRepository<DIPDbContext, ZoneParagraph, Guid>, IZoneParagraphRepository
    {
        public EfCoreZoneParagraphRepository(IDbContextProvider<DIPDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<ZoneParagraphWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(zoneParagraph => new ZoneParagraphWithNavigationProperties
                {
                    ZoneParagraph = zoneParagraph,
                    Zone = dbContext.Set<Zone>().FirstOrDefault(c => c.Id == zoneParagraph.ZoneId)
                }).FirstOrDefault();
        }

        public async Task<List<ZoneParagraphWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string subTilteEn = null,
            string subTitleAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? zoneId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, titleEn, titleAr, subTilteEn, subTitleAr, descriptionEn, descriptionAr, orderMin, orderMax, isActive, zoneId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ZoneParagraphConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<ZoneParagraphWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from zoneParagraph in (await GetDbSetAsync())
                   join zone in (await GetDbContextAsync()).Set<Zone>() on zoneParagraph.ZoneId equals zone.Id into zones
                   from zone in zones.DefaultIfEmpty()
                   select new ZoneParagraphWithNavigationProperties
                   {
                       ZoneParagraph = zoneParagraph,
                       Zone = zone
                   };
        }

        protected virtual IQueryable<ZoneParagraphWithNavigationProperties> ApplyFilter(
            IQueryable<ZoneParagraphWithNavigationProperties> query,
            string filterText,
            string titleEn = null,
            string titleAr = null,
            string subTilteEn = null,
            string subTitleAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? zoneId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.ZoneParagraph.TitleEn.Contains(filterText) || e.ZoneParagraph.TitleAr.Contains(filterText) || e.ZoneParagraph.SubTilteEn.Contains(filterText) || e.ZoneParagraph.SubTitleAr.Contains(filterText) || e.ZoneParagraph.DescriptionEn.Contains(filterText) || e.ZoneParagraph.DescriptionAr.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.ZoneParagraph.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.ZoneParagraph.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(subTilteEn), e => e.ZoneParagraph.SubTilteEn.Contains(subTilteEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(subTitleAr), e => e.ZoneParagraph.SubTitleAr.Contains(subTitleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionEn), e => e.ZoneParagraph.DescriptionEn.Contains(descriptionEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionAr), e => e.ZoneParagraph.DescriptionAr.Contains(descriptionAr))
                    .WhereIf(orderMin.HasValue, e => e.ZoneParagraph.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.ZoneParagraph.Order <= orderMax.Value)
                    .WhereIf(isActive.HasValue, e => e.ZoneParagraph.IsActive == isActive)
                    .WhereIf(zoneId != null && zoneId != Guid.Empty, e => e.Zone != null && e.Zone.Id == zoneId);
        }

        public async Task<List<ZoneParagraph>> GetListAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string subTilteEn = null,
            string subTitleAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, titleEn, titleAr, subTilteEn, subTitleAr, descriptionEn, descriptionAr, orderMin, orderMax, isActive);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ZoneParagraphConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string subTilteEn = null,
            string subTitleAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? zoneId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, titleEn, titleAr, subTilteEn, subTitleAr, descriptionEn, descriptionAr, orderMin, orderMax, isActive, zoneId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<ZoneParagraph> ApplyFilter(
            IQueryable<ZoneParagraph> query,
            string filterText,
            string titleEn = null,
            string titleAr = null,
            string subTilteEn = null,
            string subTitleAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.TitleEn.Contains(filterText) || e.TitleAr.Contains(filterText) || e.SubTilteEn.Contains(filterText) || e.SubTitleAr.Contains(filterText) || e.DescriptionEn.Contains(filterText) || e.DescriptionAr.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(subTilteEn), e => e.SubTilteEn.Contains(subTilteEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(subTitleAr), e => e.SubTitleAr.Contains(subTitleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionEn), e => e.DescriptionEn.Contains(descriptionEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionAr), e => e.DescriptionAr.Contains(descriptionAr))
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax.Value)
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive);
        }
    }
}