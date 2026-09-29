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

namespace DIP.EServices
{
    public class EfCoreEServiceRepository : EfCoreRepository<DIPDbContext, EService, Guid>, IEServiceRepository
    {
        public EfCoreEServiceRepository(IDbContextProvider<DIPDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<List<EService>> GetListAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string slug = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string image = null,
            string headerImage = null,
            string metaTitleEn = null,
            string metaTitleAr = null,
            string metaDescriptionEn = null,
            string metaDescriptionAr = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, titleEn, titleAr, slug, descriptionEn, descriptionAr, image, headerImage, metaTitleEn, metaTitleAr, metaDescriptionEn, metaDescriptionAr, orderMin, orderMax, isActive);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? EServiceConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string slug = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string image = null,
            string headerImage = null,
            string metaTitleEn = null,
            string metaTitleAr = null,
            string metaDescriptionEn = null,
            string metaDescriptionAr = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, titleEn, titleAr, slug, descriptionEn, descriptionAr, image, headerImage, metaTitleEn, metaTitleAr, metaDescriptionEn, metaDescriptionAr, orderMin, orderMax, isActive);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<EService> ApplyFilter(
            IQueryable<EService> query,
            string filterText,
            string titleEn = null,
            string titleAr = null,
            string slug = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string image = null,
            string headerImage = null,
            string metaTitleEn = null,
            string metaTitleAr = null,
            string metaDescriptionEn = null,
            string metaDescriptionAr = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.TitleEn.Contains(filterText) || e.TitleAr.Contains(filterText) || e.Slug.Contains(filterText) || e.DescriptionEn.Contains(filterText) || e.DescriptionAr.Contains(filterText) || e.Image.Contains(filterText) || e.HeaderImage.Contains(filterText) || e.MetaTitleEn.Contains(filterText) || e.MetaTitleAr.Contains(filterText) || e.MetaDescriptionEn.Contains(filterText) || e.MetaDescriptionAr.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(slug), e => e.Slug.Contains(slug))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionEn), e => e.DescriptionEn.Contains(descriptionEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionAr), e => e.DescriptionAr.Contains(descriptionAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(image), e => e.Image.Contains(image))
                    .WhereIf(!string.IsNullOrWhiteSpace(headerImage), e => e.HeaderImage.Contains(headerImage))
                    .WhereIf(!string.IsNullOrWhiteSpace(metaTitleEn), e => e.MetaTitleEn.Contains(metaTitleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(metaTitleAr), e => e.MetaTitleAr.Contains(metaTitleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(metaDescriptionEn), e => e.MetaDescriptionEn.Contains(metaDescriptionEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(metaDescriptionAr), e => e.MetaDescriptionAr.Contains(metaDescriptionAr))
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax.Value)
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive);
        }
    }
}