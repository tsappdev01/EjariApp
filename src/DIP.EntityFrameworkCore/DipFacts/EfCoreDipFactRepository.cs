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

namespace DIP.DipFacts
{
    public class EfCoreDipFactRepository : EfCoreRepository<DIPDbContext, DipFact, Guid>, IDipFactRepository
    {
        public EfCoreDipFactRepository(IDbContextProvider<DIPDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<List<DipFact>> GetListAsync(
            string filterText = null,
            string image = null,
            string titleAr = null,
            string titleEn = null,
            string descriptionAr = null,
            string descriptionEn = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, image, titleAr, titleEn, descriptionAr, descriptionEn, orderMin, orderMax, isActive);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? DipFactConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string image = null,
            string titleAr = null,
            string titleEn = null,
            string descriptionAr = null,
            string descriptionEn = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, image, titleAr, titleEn, descriptionAr, descriptionEn, orderMin, orderMax, isActive);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<DipFact> ApplyFilter(
            IQueryable<DipFact> query,
            string filterText,
            string image = null,
            string titleAr = null,
            string titleEn = null,
            string descriptionAr = null,
            string descriptionEn = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Image.Contains(filterText) || e.TitleAr.Contains(filterText) || e.TitleEn.Contains(filterText) || e.DescriptionAr.Contains(filterText) || e.DescriptionEn.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(image), e => e.Image.Contains(image))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionAr), e => e.DescriptionAr.Contains(descriptionAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(descriptionEn), e => e.DescriptionEn.Contains(descriptionEn))
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax.Value)
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive);
        }
    }
}