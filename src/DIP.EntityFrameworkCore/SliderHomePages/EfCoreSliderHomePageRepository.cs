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

namespace DIP.SliderHomePages
{
    public class EfCoreSliderHomePageRepository : EfCoreRepository<DIPDbContext, SliderHomePage, Guid>, ISliderHomePageRepository
    {
        public EfCoreSliderHomePageRepository(IDbContextProvider<DIPDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task<List<SliderHomePage>> GetListAsync(
            string? filterText = null,
            string? titleAr = null,
            string? titleEn = null,
            string? descriptionAr = null,
            string? descriptionEn = null,
            string? buttonTitleAr = null,
            string? buttonTitleEn = null,
            string? buttonUrlEn = null,
            string? buttonUrlAr = null,
            string? image = null,
            string? youtubeUrl = null,
            bool? isActive = null,
            int? orderMin = null,
            int? orderMax = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, titleAr, titleEn, descriptionAr, descriptionEn, buttonTitleAr, buttonTitleEn, buttonUrlEn, buttonUrlAr, image, youtubeUrl, isActive, orderMin, orderMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? SliderHomePageConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? titleAr = null,
            string? titleEn = null,
            string? descriptionAr = null,
            string? descriptionEn = null,
            string? buttonTitleAr = null,
            string? buttonTitleEn = null,
            string? buttonUrlEn = null,
            string? buttonUrlAr = null,
            string? image = null,
            string? youtubeUrl = null,
            bool? isActive = null,
            int? orderMin = null,
            int? orderMax = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, titleAr, titleEn, descriptionAr, descriptionEn, buttonTitleAr, buttonTitleEn, buttonUrlEn, buttonUrlAr, image, youtubeUrl, isActive, orderMin, orderMax);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<SliderHomePage> ApplyFilter(
            IQueryable<SliderHomePage> query,
            string? filterText = null,
            string? titleAr = null,
            string? titleEn = null,
            string? descriptionAr = null,
            string? descriptionEn = null,
            string? buttonTitleAr = null,
            string? buttonTitleEn = null,
            string? buttonUrlEn = null,
            string? buttonUrlAr = null,
            string? image = null,
            string? youtubeUrl = null,
            bool? isActive = null,
            int? orderMin = null,
            int? orderMax = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.TitleAr!.Contains(filterText!) || e.TitleEn!.Contains(filterText!) || e.DescriptionAr!.Contains(filterText!) || e.DescriptionEn!.Contains(filterText!) || e.ButtonTitleAr!.Contains(filterText!) || e.ButtonTitleEn!.Contains(filterText!) || e.ButtonUrlEn!.Contains(filterText!) || e.ButtonUrlAr!.Contains(filterText!) || e.Image!.Contains(filterText!) || e.YoutubeUrl!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionAr), e => e.DescriptionAr.Contains(descriptionAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionEn), e => e.DescriptionEn.Contains(descriptionEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(buttonTitleAr), e => e.ButtonTitleAr.Contains(buttonTitleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(buttonTitleEn), e => e.ButtonTitleEn.Contains(buttonTitleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(buttonUrlEn), e => e.ButtonUrlEn.Contains(buttonUrlEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(buttonUrlAr), e => e.ButtonUrlAr.Contains(buttonUrlAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(image), e => e.Image.Contains(image))
                    .WhereIf(!string.IsNullOrWhiteSpace(youtubeUrl), e => e.YoutubeUrl.Contains(youtubeUrl))
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive)
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin!.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax!.Value);
        }
    }
}