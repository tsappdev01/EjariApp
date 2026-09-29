using DIP.Categories;
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

namespace DIP.SubCategories
{
    public class EfCoreSubCategoryRepository : EfCoreRepository<DIPDbContext, SubCategory, Guid>, ISubCategoryRepository
    {
        public EfCoreSubCategoryRepository(IDbContextProvider<DIPDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<SubCategoryWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(subCategory => new SubCategoryWithNavigationProperties
                {
                    SubCategory = subCategory,
                    Category = dbContext.Set<Category>().FirstOrDefault(c => c.Id == subCategory.CategoryId)
                }).FirstOrDefault();
        }

        public async Task<List<SubCategoryWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string titleAr = null,
            string titleEn = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isFeature = null,
            bool? isActive = null,
            Guid? categoryId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, titleAr, titleEn, orderMin, orderMax, isFeature, isActive, categoryId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? SubCategoryConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<SubCategoryWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from subCategory in (await GetDbSetAsync())
                   join category in (await GetDbContextAsync()).Set<Category>() on subCategory.CategoryId equals category.Id into categories
                   from category in categories.DefaultIfEmpty()
                   select new SubCategoryWithNavigationProperties
                   {
                       SubCategory = subCategory,
                       Category = category
                   };
        }

        protected virtual IQueryable<SubCategoryWithNavigationProperties> ApplyFilter(
            IQueryable<SubCategoryWithNavigationProperties> query,
            string filterText,
            string titleAr = null,
            string titleEn = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isFeature = null,
            bool? isActive = null,
            Guid? categoryId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.SubCategory.TitleAr.Contains(filterText) || e.SubCategory.TitleEn.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.SubCategory.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.SubCategory.TitleEn.Contains(titleEn))
                    .WhereIf(orderMin.HasValue, e => e.SubCategory.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.SubCategory.Order <= orderMax.Value)
                    .WhereIf(isFeature.HasValue, e => e.SubCategory.IsFeature == isFeature)
                    .WhereIf(isActive.HasValue, e => e.SubCategory.IsActive == isActive)
                    .WhereIf(categoryId != null && categoryId != Guid.Empty, e => e.Category != null && e.Category.Id == categoryId);
        }

        public async Task<List<SubCategory>> GetListAsync(
            string filterText = null,
            string titleAr = null,
            string titleEn = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isFeature = null,
            bool? isActive = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, titleAr, titleEn, orderMin, orderMax, isFeature, isActive);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? SubCategoryConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string titleAr = null,
            string titleEn = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isFeature = null,
            bool? isActive = null,
            Guid? categoryId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, titleAr, titleEn, orderMin, orderMax, isFeature, isActive, categoryId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<SubCategory> ApplyFilter(
            IQueryable<SubCategory> query,
            string filterText,
            string titleAr = null,
            string titleEn = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isFeature = null,
            bool? isActive = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.TitleAr.Contains(filterText) || e.TitleEn.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.TitleEn.Contains(titleEn))
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax.Value)
                    .WhereIf(isFeature.HasValue, e => e.IsFeature == isFeature)
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive);
        }
    }
}