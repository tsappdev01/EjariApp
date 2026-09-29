using DIP.TimeLineCategories;
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

namespace DIP.TimeLines
{
    public class EfCoreTimeLineRepository : EfCoreRepository<DIPDbContext, TimeLine, Guid>, ITimeLineRepository
    {
        public EfCoreTimeLineRepository(IDbContextProvider<DIPDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<TimeLineWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(timeLine => new TimeLineWithNavigationProperties
                {
                    TimeLine = timeLine,
                    TimeLineCategory = dbContext.Set<TimeLineCategory>().FirstOrDefault(c => c.Id == timeLine.TimeLineCategoryId)
                }).FirstOrDefault();
        }

        public async Task<List<TimeLineWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string image = null,
            DateTime? timeLineDateMin = null,
            DateTime? timeLineDateMax = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? timeLineCategoryId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, titleEn, titleAr, descriptionEn, descriptionAr, image, timeLineDateMin, timeLineDateMax, orderMin, orderMax, isActive, timeLineCategoryId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? TimeLineConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<TimeLineWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from timeLine in (await GetDbSetAsync())
                   join timeLineCategory in (await GetDbContextAsync()).Set<TimeLineCategory>() on timeLine.TimeLineCategoryId equals timeLineCategory.Id into timeLineCategories
                   from timeLineCategory in timeLineCategories.DefaultIfEmpty()
                   select new TimeLineWithNavigationProperties
                   {
                       TimeLine = timeLine,
                       TimeLineCategory = timeLineCategory
                   };
        }

        protected virtual IQueryable<TimeLineWithNavigationProperties> ApplyFilter(
            IQueryable<TimeLineWithNavigationProperties> query,
            string filterText,
            string titleEn = null,
            string titleAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string image = null,
            DateTime? timeLineDateMin = null,
            DateTime? timeLineDateMax = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? timeLineCategoryId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.TimeLine.TitleEn.Contains(filterText) || e.TimeLine.TitleAr.Contains(filterText) || e.TimeLine.DescriptionEn.Contains(filterText) || e.TimeLine.DescriptionAr.Contains(filterText) || e.TimeLine.Image.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.TimeLine.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.TimeLine.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionEn), e => e.TimeLine.DescriptionEn.Contains(descriptionEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionAr), e => e.TimeLine.DescriptionAr.Contains(descriptionAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(image), e => e.TimeLine.Image.Contains(image))
                    .WhereIf(timeLineDateMin.HasValue, e => e.TimeLine.TimeLineDate >= timeLineDateMin.Value)
                    .WhereIf(timeLineDateMax.HasValue, e => e.TimeLine.TimeLineDate <= timeLineDateMax.Value)
                    .WhereIf(orderMin.HasValue, e => e.TimeLine.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.TimeLine.Order <= orderMax.Value)
                    .WhereIf(isActive.HasValue, e => e.TimeLine.IsActive == isActive)
                    .WhereIf(timeLineCategoryId != null && timeLineCategoryId != Guid.Empty, e => e.TimeLineCategory != null && e.TimeLineCategory.Id == timeLineCategoryId);
        }

        public async Task<List<TimeLine>> GetListAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string image = null,
            DateTime? timeLineDateMin = null,
            DateTime? timeLineDateMax = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, titleEn, titleAr, descriptionEn, descriptionAr, image, timeLineDateMin, timeLineDateMax, orderMin, orderMax, isActive);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? TimeLineConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string image = null,
            DateTime? timeLineDateMin = null,
            DateTime? timeLineDateMax = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? timeLineCategoryId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, titleEn, titleAr, descriptionEn, descriptionAr, image, timeLineDateMin, timeLineDateMax, orderMin, orderMax, isActive, timeLineCategoryId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<TimeLine> ApplyFilter(
            IQueryable<TimeLine> query,
            string filterText,
            string titleEn = null,
            string titleAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string image = null,
            DateTime? timeLineDateMin = null,
            DateTime? timeLineDateMax = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.TitleEn.Contains(filterText) || e.TitleAr.Contains(filterText) || e.DescriptionEn.Contains(filterText) || e.DescriptionAr.Contains(filterText) || e.Image.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionEn), e => e.DescriptionEn.Contains(descriptionEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionAr), e => e.DescriptionAr.Contains(descriptionAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(image), e => e.Image.Contains(image))
                    .WhereIf(timeLineDateMin.HasValue, e => e.TimeLineDate >= timeLineDateMin.Value)
                    .WhereIf(timeLineDateMax.HasValue, e => e.TimeLineDate <= timeLineDateMax.Value)
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax.Value)
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive);
        }
    }
}