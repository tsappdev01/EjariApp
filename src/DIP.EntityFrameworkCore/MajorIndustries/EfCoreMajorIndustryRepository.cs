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

namespace DIP.MajorIndustries
{
    public class EfCoreMajorIndustryRepository : EfCoreRepository<DIPDbContext, MajorIndustry, Guid>, IMajorIndustryRepository
    {
        public EfCoreMajorIndustryRepository(IDbContextProvider<DIPDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<MajorIndustryWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(majorIndustry => new MajorIndustryWithNavigationProperties
                {
                    MajorIndustry = majorIndustry,
                    Zone = dbContext.Set<Zone>().FirstOrDefault(c => c.Id == majorIndustry.ZoneId)
                }).FirstOrDefault();
        }

        public async Task<List<MajorIndustryWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string image = null,
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
            query = ApplyFilter(query, filterText, titleEn, titleAr, image, orderMin, orderMax, isActive, zoneId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? MajorIndustryConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<MajorIndustryWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from majorIndustry in (await GetDbSetAsync())
                   join zone in (await GetDbContextAsync()).Set<Zone>() on majorIndustry.ZoneId equals zone.Id into zones
                   from zone in zones.DefaultIfEmpty()
                   select new MajorIndustryWithNavigationProperties
                   {
                       MajorIndustry = majorIndustry,
                       Zone = zone
                   };
        }

        protected virtual IQueryable<MajorIndustryWithNavigationProperties> ApplyFilter(
            IQueryable<MajorIndustryWithNavigationProperties> query,
            string filterText,
            string titleEn = null,
            string titleAr = null,
            string image = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? zoneId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.MajorIndustry.TitleEn.Contains(filterText) || e.MajorIndustry.TitleAr.Contains(filterText) || e.MajorIndustry.Image.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.MajorIndustry.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.MajorIndustry.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(image), e => e.MajorIndustry.Image.Contains(image))
                    .WhereIf(orderMin.HasValue, e => e.MajorIndustry.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.MajorIndustry.Order <= orderMax.Value)
                    .WhereIf(isActive.HasValue, e => e.MajorIndustry.IsActive == isActive)
                    .WhereIf(zoneId != null && zoneId != Guid.Empty, e => e.Zone != null && e.Zone.Id == zoneId);
        }

        public async Task<List<MajorIndustry>> GetListAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string image = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, titleEn, titleAr, image, orderMin, orderMax, isActive);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? MajorIndustryConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string image = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? zoneId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, titleEn, titleAr, image, orderMin, orderMax, isActive, zoneId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<MajorIndustry> ApplyFilter(
            IQueryable<MajorIndustry> query,
            string filterText,
            string titleEn = null,
            string titleAr = null,
            string image = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.TitleEn.Contains(filterText) || e.TitleAr.Contains(filterText) || e.Image.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(image), e => e.Image.Contains(image))
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax.Value)
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive);
        }
    }
}