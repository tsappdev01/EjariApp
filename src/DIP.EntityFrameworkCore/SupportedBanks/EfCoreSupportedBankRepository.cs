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

namespace DIP.SupportedBanks
{
    public class EfCoreSupportedBankRepository : EfCoreRepository<DIPDbContext, SupportedBank, Guid>, ISupportedBankRepository
    {
        public EfCoreSupportedBankRepository(IDbContextProvider<DIPDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<List<SupportedBank>> GetListAsync(
            string filterText = null,
            string titleAr = null,
            string titleEn = null,
            bool? isActive = null,
            int? orderMin = null,
            int? orderMax = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, titleAr, titleEn, isActive, orderMin, orderMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? SupportedBankConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string titleAr = null,
            string titleEn = null,
            bool? isActive = null,
            int? orderMin = null,
            int? orderMax = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, titleAr, titleEn, isActive, orderMin, orderMax);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<SupportedBank> ApplyFilter(
            IQueryable<SupportedBank> query,
            string filterText,
            string titleAr = null,
            string titleEn = null,
            bool? isActive = null,
            int? orderMin = null,
            int? orderMax = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.TitleAr.Contains(filterText) || e.TitleEn.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.TitleEn.Contains(titleEn))
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive)
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax.Value);
        }
    }
}