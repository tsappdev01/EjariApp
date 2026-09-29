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

namespace DIP.ContactForms
{
    public class EfCoreContactFormRepository : EfCoreRepository<DIPDbContext, ContactForm, Guid>, IContactFormRepository
    {
        public EfCoreContactFormRepository(IDbContextProvider<DIPDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<List<ContactForm>> GetListAsync(
            string filterText = null,
            string fullName = null,
            string email = null,
            string subject = null,
            string message = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, fullName, email, subject, message);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ContactFormConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string fullName = null,
            string email = null,
            string subject = null,
            string message = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, fullName, email, subject, message);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<ContactForm> ApplyFilter(
            IQueryable<ContactForm> query,
            string filterText,
            string fullName = null,
            string email = null,
            string subject = null,
            string message = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.FullName.Contains(filterText) || e.Email.Contains(filterText) || e.Subject.Contains(filterText) || e.Message.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(fullName), e => e.FullName.Contains(fullName))
                    .WhereIf(!string.IsNullOrWhiteSpace(email), e => e.Email.Contains(email))
                    .WhereIf(!string.IsNullOrWhiteSpace(subject), e => e.Subject.Contains(subject))
                    .WhereIf(!string.IsNullOrWhiteSpace(message), e => e.Message.Contains(message));
        }
    }
}