using DIP.EFormServices;
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

namespace DIP.EFormServiceSubCategories
{
    public class EfCoreEFormServiceSubCategoryRepository : EfCoreRepository<DIPDbContext, EFormServiceSubCategory, Guid>, IEFormServiceSubCategoryRepository
    {
        public EfCoreEFormServiceSubCategoryRepository(IDbContextProvider<DIPDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<EFormServiceSubCategoryWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(eFormServiceSubCategory => new EFormServiceSubCategoryWithNavigationProperties
                {
                    EFormServiceSubCategory = eFormServiceSubCategory,
                    EFormService = dbContext.Set<EFormService>().FirstOrDefault(c => c.Id == eFormServiceSubCategory.EFormServiceId)
                }).FirstOrDefault();
        }

        public async Task<List<EFormServiceSubCategoryWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? eFormServiceId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, titleEn, titleAr, file, orderMin, orderMax, isActive, eFormServiceId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? EFormServiceSubCategoryConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<EFormServiceSubCategoryWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from eFormServiceSubCategory in (await GetDbSetAsync())
                   join eFormService in (await GetDbContextAsync()).Set<EFormService>() on eFormServiceSubCategory.EFormServiceId equals eFormService.Id into eFormServices
                   from eFormService in eFormServices.DefaultIfEmpty()
                   select new EFormServiceSubCategoryWithNavigationProperties
                   {
                       EFormServiceSubCategory = eFormServiceSubCategory,
                       EFormService = eFormService
                   };
        }

        protected virtual IQueryable<EFormServiceSubCategoryWithNavigationProperties> ApplyFilter(
            IQueryable<EFormServiceSubCategoryWithNavigationProperties> query,
            string filterText,
            string titleEn = null,
            string titleAr = null,
            string file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? eFormServiceId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.EFormServiceSubCategory.TitleEn.Contains(filterText) || e.EFormServiceSubCategory.TitleAr.Contains(filterText) || e.EFormServiceSubCategory.File.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.EFormServiceSubCategory.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.EFormServiceSubCategory.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(file), e => e.EFormServiceSubCategory.File.Contains(file))
                    .WhereIf(orderMin.HasValue, e => e.EFormServiceSubCategory.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.EFormServiceSubCategory.Order <= orderMax.Value)
                    .WhereIf(isActive.HasValue, e => e.EFormServiceSubCategory.IsActive == isActive)
                    .WhereIf(eFormServiceId != null && eFormServiceId != Guid.Empty, e => e.EFormService != null && e.EFormService.Id == eFormServiceId);
        }

        public async Task<List<EFormServiceSubCategory>> GetListAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, titleEn, titleAr, file, orderMin, orderMax, isActive);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? EFormServiceSubCategoryConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? eFormServiceId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, titleEn, titleAr, file, orderMin, orderMax, isActive, eFormServiceId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<EFormServiceSubCategory> ApplyFilter(
            IQueryable<EFormServiceSubCategory> query,
            string filterText,
            string titleEn = null,
            string titleAr = null,
            string file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.TitleEn.Contains(filterText) || e.TitleAr.Contains(filterText) || e.File.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(file), e => e.File.Contains(file))
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax.Value)
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive);
        }
    }
}