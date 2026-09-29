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

namespace DIP.Commercials
{
    public partial class EfCoreCommercialRepository : EfCoreRepository<DIPDbContext, Commercial, Guid>, ICommercialRepository
    {
        public EfCoreCommercialRepository(IDbContextProvider<DIPDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<CommercialWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(commercial => new CommercialWithNavigationProperties
                {
                    Commercial = commercial,
                    SubCategory = dbContext.Set<SubCategory>().FirstOrDefault(c => c.Id == commercial.SubCategoryId)
                }).FirstOrDefault();
        }

        public async Task<List<CommercialWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string plotNo = null,
            string activityEn = null,
            string activityAr = null,
            string phone = null,
            string fax = null,
            string makaniNo = null,
            bool? isActive = null,
            int? orderMin = null,
            int? orderMax = null,
            Guid? subCategoryId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, titleEn, titleAr, plotNo, activityEn, activityAr, phone, fax, makaniNo, isActive, orderMin, orderMax, subCategoryId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? CommercialConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }
        public async Task<List<CommercialWithNavigationProperties>> GetListWithPlotNoWithNavigationPropertiesAsync(
    string filterText = null,
    string titleEn = null,
    string titleAr = null,
    string plotNo = null,
    string activityEn = null,
    string activityAr = null,
    string phone = null,
    string fax = null,
    string makaniNo = null,
    bool? isActive = null,
    int? orderMin = null,
    int? orderMax = null,
    Guid? subCategoryId = null,
    string sorting = null,
    int maxResultCount = int.MaxValue,
    int skipCount = 0,
    CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilterWithPlotNo(query, filterText, titleEn, titleAr, plotNo, activityEn, activityAr, phone, fax, makaniNo, isActive, orderMin, orderMax, subCategoryId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? CommercialConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }
        protected virtual async Task<IQueryable<CommercialWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from commercial in (await GetDbSetAsync())
                   join subCategory in (await GetDbContextAsync()).Set<SubCategory>() on commercial.SubCategoryId equals subCategory.Id into subCategories
                   from subCategory in subCategories.DefaultIfEmpty()
                   select new CommercialWithNavigationProperties
                   {
                       Commercial = commercial,
                       SubCategory = subCategory
                   };
        }

        protected virtual IQueryable<CommercialWithNavigationProperties> ApplyFilter(
            IQueryable<CommercialWithNavigationProperties> query,
            string filterText,
            string titleEn = null,
            string titleAr = null,
            string plotNo = null,
            string activityEn = null,
            string activityAr = null,
            string phone = null,
            string fax = null,
            string makaniNo = null,
            bool? isActive = null,
            int? orderMin = null,
            int? orderMax = null,
            Guid? subCategoryId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Commercial.TitleEn.Contains(filterText) || e.Commercial.TitleAr.Contains(filterText) || e.Commercial.PlotNo.Contains(filterText) || e.Commercial.ActivityEn.Contains(filterText) || e.Commercial.ActivityAr.Contains(filterText) || e.Commercial.Phone.Contains(filterText) || e.Commercial.Fax.Contains(filterText) || e.Commercial.MakaniNo.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.Commercial.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.Commercial.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(plotNo), e => e.Commercial.PlotNo.Contains(plotNo))
                    .WhereIf(!string.IsNullOrWhiteSpace(activityEn), e => e.Commercial.ActivityEn.Contains(activityEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(activityAr), e => e.Commercial.ActivityAr.Contains(activityAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(phone), e => e.Commercial.Phone.Contains(phone))
                    .WhereIf(!string.IsNullOrWhiteSpace(fax), e => e.Commercial.Fax.Contains(fax))
                    .WhereIf(!string.IsNullOrWhiteSpace(makaniNo), e => e.Commercial.MakaniNo.Contains(makaniNo))
                    .WhereIf(isActive.HasValue, e => e.Commercial.IsActive == isActive)
                    .WhereIf(orderMin.HasValue, e => e.Commercial.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.Commercial.Order <= orderMax.Value)
                    .WhereIf(subCategoryId != null && subCategoryId != Guid.Empty, e => e.SubCategory != null && e.SubCategory.Id == subCategoryId);
        }

        protected virtual IQueryable<CommercialWithNavigationProperties> ApplyFilterWithPlotNo(
        IQueryable<CommercialWithNavigationProperties> query,
        string filterText,
        string titleEn = null,
        string titleAr = null,
        string plotNo = null,
        string activityEn = null,
        string activityAr = null,
        string phone = null,
        string fax = null,
        string makaniNo = null,
        bool? isActive = null,
        int? orderMin = null,
        int? orderMax = null,
        Guid? subCategoryId = null)
        {
            return query
                    .WhereIf(true, e => !string.IsNullOrEmpty(e.Commercial.PlotNo) || !string.IsNullOrEmpty(e.Commercial.PlotNo))
                      .WhereIf(true, e => string.IsNullOrWhiteSpace(e.Commercial.MakaniNo) || string.IsNullOrEmpty(e.Commercial.MakaniNo));
        }

        public async Task<List<Commercial>> GetListAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string plotNo = null,
            string activityEn = null,
            string activityAr = null,
            string phone = null,
            string fax = null,
            string makaniNo = null,
            bool? isActive = null,
            int? orderMin = null,
            int? orderMax = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, titleEn, titleAr, plotNo, activityEn, activityAr, phone, fax, makaniNo, isActive, orderMin, orderMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? CommercialConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string plotNo = null,
            string activityEn = null,
            string activityAr = null,
            string phone = null,
            string fax = null,
            string makaniNo = null,
            bool? isActive = null,
            int? orderMin = null,
            int? orderMax = null,
            Guid? subCategoryId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, titleEn, titleAr, plotNo, activityEn, activityAr, phone, fax, makaniNo, isActive, orderMin, orderMax, subCategoryId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Commercial> ApplyFilter(
            IQueryable<Commercial> query,
            string filterText,
            string titleEn = null,
            string titleAr = null,
            string plotNo = null,
            string activityEn = null,
            string activityAr = null,
            string phone = null,
            string fax = null,
            string makaniNo = null,
            bool? isActive = null,
            int? orderMin = null,
            int? orderMax = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.TitleEn.Contains(filterText) || e.TitleAr.Contains(filterText) || e.PlotNo.Contains(filterText) || e.ActivityEn.Contains(filterText) || e.ActivityAr.Contains(filterText) || e.Phone.Contains(filterText) || e.Fax.Contains(filterText) || e.MakaniNo.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(plotNo), e => e.PlotNo.Contains(plotNo))
                    .WhereIf(!string.IsNullOrWhiteSpace(activityEn), e => e.ActivityEn.Contains(activityEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(activityAr), e => e.ActivityAr.Contains(activityAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(phone), e => e.Phone.Contains(phone))
                    .WhereIf(!string.IsNullOrWhiteSpace(fax), e => e.Fax.Contains(fax))
                    .WhereIf(!string.IsNullOrWhiteSpace(makaniNo), e => e.MakaniNo.Contains(makaniNo))
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive)
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax.Value);
        }
    }
}