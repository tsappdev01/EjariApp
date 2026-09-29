using DIP.PageInfos;
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

namespace DIP.PageInfoSections
{
    public class EfCorePageInfoSectionRepository : EfCoreRepository<DIPDbContext, PageInfoSection, Guid>, IPageInfoSectionRepository
    {
        public EfCorePageInfoSectionRepository(IDbContextProvider<DIPDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<PageInfoSectionWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(pageInfoSection => new PageInfoSectionWithNavigationProperties
                {
                    PageInfoSection = pageInfoSection,
                    PageInfo = dbContext.Set<PageInfo>().FirstOrDefault(c => c.Id == pageInfoSection.PageInfoId)
                }).FirstOrDefault();
        }

        public async Task<List<PageInfoSectionWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string subTitleEn = null,
            string subTitleAr = null,
            string summaryEn = null,
            string summaryAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string pageSectionMedia = null,
            string youtubeUrl = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? pageInfoId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, titleEn, titleAr, subTitleEn, subTitleAr, summaryEn, summaryAr, descriptionEn, descriptionAr, pageSectionMedia, youtubeUrl, orderMin, orderMax, isActive, pageInfoId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PageInfoSectionConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<PageInfoSectionWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from pageInfoSection in (await GetDbSetAsync())
                   join pageInfo in (await GetDbContextAsync()).Set<PageInfo>() on pageInfoSection.PageInfoId equals pageInfo.Id into pageInfos
                   from pageInfo in pageInfos.DefaultIfEmpty()
                   select new PageInfoSectionWithNavigationProperties
                   {
                       PageInfoSection = pageInfoSection,
                       PageInfo = pageInfo
                   };
        }

        protected virtual IQueryable<PageInfoSectionWithNavigationProperties> ApplyFilter(
            IQueryable<PageInfoSectionWithNavigationProperties> query,
            string filterText,
            string titleEn = null,
            string titleAr = null,
            string subTitleEn = null,
            string subTitleAr = null,
            string summaryEn = null,
            string summaryAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string pageSectionMedia = null,
            string youtubeUrl = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? pageInfoId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.PageInfoSection.TitleEn.Contains(filterText) || e.PageInfoSection.TitleAr.Contains(filterText) || e.PageInfoSection.SubTitleEn.Contains(filterText) || e.PageInfoSection.SubTitleAr.Contains(filterText) || e.PageInfoSection.SummaryEn.Contains(filterText) || e.PageInfoSection.SummaryAr.Contains(filterText) || e.PageInfoSection.DescriptionEn.Contains(filterText) || e.PageInfoSection.DescriptionAr.Contains(filterText) || e.PageInfoSection.PageSectionMedia.Contains(filterText) || e.PageInfoSection.YoutubeUrl.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.PageInfoSection.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.PageInfoSection.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(subTitleEn), e => e.PageInfoSection.SubTitleEn.Contains(subTitleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(subTitleAr), e => e.PageInfoSection.SubTitleAr.Contains(subTitleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(summaryEn), e => e.PageInfoSection.SummaryEn.Contains(summaryEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(summaryAr), e => e.PageInfoSection.SummaryAr.Contains(summaryAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionEn), e => e.PageInfoSection.DescriptionEn.Contains(descriptionEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionAr), e => e.PageInfoSection.DescriptionAr.Contains(descriptionAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(pageSectionMedia), e => e.PageInfoSection.PageSectionMedia.Contains(pageSectionMedia))
                    .WhereIf(!string.IsNullOrWhiteSpace(youtubeUrl), e => e.PageInfoSection.YoutubeUrl.Contains(youtubeUrl))
                    .WhereIf(orderMin.HasValue, e => e.PageInfoSection.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.PageInfoSection.Order <= orderMax.Value)
                    .WhereIf(isActive.HasValue, e => e.PageInfoSection.IsActive == isActive)
                    .WhereIf(pageInfoId != null && pageInfoId != Guid.Empty, e => e.PageInfo != null && e.PageInfo.Id == pageInfoId);
        }

        public async Task<List<PageInfoSection>> GetListAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string subTitleEn = null,
            string subTitleAr = null,
            string summaryEn = null,
            string summaryAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string pageSectionMedia = null,
            string youtubeUrl = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, titleEn, titleAr, subTitleEn, subTitleAr, summaryEn, summaryAr, descriptionEn, descriptionAr, pageSectionMedia, youtubeUrl, orderMin, orderMax, isActive);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PageInfoSectionConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string subTitleEn = null,
            string subTitleAr = null,
            string summaryEn = null,
            string summaryAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string pageSectionMedia = null,
            string youtubeUrl = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? pageInfoId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, titleEn, titleAr, subTitleEn, subTitleAr, summaryEn, summaryAr, descriptionEn, descriptionAr, pageSectionMedia, youtubeUrl, orderMin, orderMax, isActive, pageInfoId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<PageInfoSection> ApplyFilter(
            IQueryable<PageInfoSection> query,
            string filterText,
            string titleEn = null,
            string titleAr = null,
            string subTitleEn = null,
            string subTitleAr = null,
            string summaryEn = null,
            string summaryAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string pageSectionMedia = null,
            string youtubeUrl = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.TitleEn.Contains(filterText) || e.TitleAr.Contains(filterText) || e.SubTitleEn.Contains(filterText) || e.SubTitleAr.Contains(filterText) || e.SummaryEn.Contains(filterText) || e.SummaryAr.Contains(filterText) || e.DescriptionEn.Contains(filterText) || e.DescriptionAr.Contains(filterText) || e.PageSectionMedia.Contains(filterText) || e.YoutubeUrl.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(subTitleEn), e => e.SubTitleEn.Contains(subTitleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(subTitleAr), e => e.SubTitleAr.Contains(subTitleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(summaryEn), e => e.SummaryEn.Contains(summaryEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(summaryAr), e => e.SummaryAr.Contains(summaryAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionEn), e => e.DescriptionEn.Contains(descriptionEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionAr), e => e.DescriptionAr.Contains(descriptionAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(pageSectionMedia), e => e.PageSectionMedia.Contains(pageSectionMedia))
                    .WhereIf(!string.IsNullOrWhiteSpace(youtubeUrl), e => e.YoutubeUrl.Contains(youtubeUrl))
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax.Value)
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive);
        }
    }
}