using DIP.SubCategories;
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
using DIP.Categories;

namespace DIP.Commercials
{
    public partial class EfCoreCommercialRepository
    {

        public async Task<List<CommercialWithDetails>> GetListWithDetailsAsync(
             string filterText = null,
             string titleEn = null,
             string titleAr = null,
             string plotNo = null,
             string activityEn = null,
             string activityAr = null,
             string phone = null,
             string fax = null,
             bool? isActive = null,
             int? orderMin = null,
             int? orderMax = null,
             List<Guid> subCategoryIds = null,
             char? startWithLetter = null,
             string sorting = null,
             int maxResultCount = int.MaxValue,
             int skipCount = 0,
             CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForDetailsAsync();
            query = ApplyFilterDetails(query, filterText, titleEn, titleAr, plotNo, activityEn, activityAr, phone, fax, isActive, orderMin, orderMax, subCategoryIds, startWithLetter);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? CommercialConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<List<CommercialWithDetails>> GetListWithDetailsByTextAsync(
     string filterText = null,
     string sorting = null,
     int maxResultCount = int.MaxValue,
     int skipCount = 0,
     CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForDetailsAsync();
            query = ApplyFilterDetailsByText(query, filterText);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? CommercialConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountDetailsAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string plotNo = null,
            string activityEn = null,
            string activityAr = null,
            string phone = null,
            string fax = null,
            bool? isActive = null,
            int? orderMin = null,
            int? orderMax = null,
            List<Guid> subCategoryIds = null,
            char? startWithLetter = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForDetailsAsync();
            query = ApplyFilterDetails(query, filterText, titleEn, titleAr, plotNo, activityEn, activityAr, phone, fax, isActive, orderMin, orderMax, subCategoryIds, startWithLetter);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountDetailsByTextAsync(
           string filterText = null,
           CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForDetailsAsync();
            query = ApplyFilterDetailsByText(query, filterText);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }
        protected virtual async Task<IQueryable<CommercialWithDetails>> GetQueryForDetailsAsync()
        {
            return from commercial in (await GetDbSetAsync())
                   join subCategory in (await GetDbContextAsync()).Set<SubCategory>() on commercial.SubCategoryId equals subCategory.Id into subCategories
                   from subCategory in subCategories.DefaultIfEmpty()
                   join category in (await GetDbContextAsync()).Set<Category>() on subCategory.CategoryId equals category.Id into categories
                   from category in categories.DefaultIfEmpty()
                   select new CommercialWithDetails
                   {
                       Commercial = commercial,
                       SubCategory = subCategory,
                       Category = category
                   };
        }

        protected virtual IQueryable<CommercialWithDetails> ApplyFilterDetails(
            IQueryable<CommercialWithDetails> query,
            string filterText,
            string titleEn = null,
            string titleAr = null,
            string plotNo = null,
            string activityEn = null,
            string activityAr = null,
            string phone = null,
            string fax = null,
            bool? isActive = null,
            int? orderMin = null,
            int? orderMax = null,
            List<Guid> subCategoryIds = null,
            char? startWithLetter = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Commercial.TitleEn.Contains(filterText) || e.Commercial.TitleAr.Contains(filterText) || e.Commercial.PlotNo.Contains(filterText) || e.Commercial.ActivityEn.Contains(filterText) || e.Commercial.ActivityAr.Contains(filterText) || e.Commercial.Phone.Contains(filterText) || e.Commercial.Fax.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.Commercial.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.Commercial.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(plotNo), e => e.Commercial.PlotNo.Contains(plotNo))
                    .WhereIf(!string.IsNullOrWhiteSpace(activityEn), e => e.Commercial.ActivityEn.Contains(activityEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(activityAr), e => e.Commercial.ActivityAr.Contains(activityAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(phone), e => e.Commercial.Phone.Contains(phone))
                    .WhereIf(!string.IsNullOrWhiteSpace(fax), e => e.Commercial.Fax.Contains(fax))
                    .WhereIf(isActive.HasValue, e => e.Commercial.IsActive == isActive)
                    .WhereIf(orderMin.HasValue, e => e.Commercial.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.Commercial.Order <= orderMax.Value)
                    .WhereIf(!subCategoryIds.IsNullOrEmpty(), e => e.SubCategory != null && subCategoryIds.Contains(e.SubCategory.Id))
                    .WhereIf(startWithLetter != null && startWithLetter != ' ', e => e.Commercial.TitleEn != null && e.Commercial.TitleEn.ToLower().StartsWith(startWithLetter.Value.ToString().ToLower()));
        }

        protected virtual IQueryable<CommercialWithDetails> ApplyFilterDetailsByText(
    IQueryable<CommercialWithDetails> query,
    string filterText)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Commercial.TitleEn.Contains(filterText) || e.Commercial.TitleAr.Contains(filterText) || e.Commercial.ActivityEn.Contains(filterText) || e.Commercial.ActivityAr.Contains(filterText) || e.Category.TitleEn.Contains(filterText) || e.Category.TitleAr.Contains(filterText) || e.SubCategory.TitleEn.Contains(filterText) || e.SubCategory.TitleAr.Contains(filterText));
        }

    }
}