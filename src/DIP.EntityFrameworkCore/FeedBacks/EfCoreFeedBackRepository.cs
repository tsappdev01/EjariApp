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

namespace DIP.FeedBacks
{
    public class EfCoreFeedBackRepository : EfCoreRepository<DIPDbContext, FeedBack, Guid>, IFeedBackRepository
    {
        public EfCoreFeedBackRepository(IDbContextProvider<DIPDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<List<FeedBack>> GetListAsync(
            string filterText = null,
            string subject = null,
            string companyName = null,
            string plotNo = null,
            string plotCategory = null,
            string contactPersonName = null,
            string emailId = null,
            string mobileNumber = null,
            string department = null,
            string categoryName = null,
            string description = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, subject, companyName, plotNo, plotCategory, contactPersonName, emailId, mobileNumber, department, categoryName, description);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? FeedBackConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string subject = null,
            string companyName = null,
            string plotNo = null,
            string plotCategory = null,
            string contactPersonName = null,
            string emailId = null,
            string mobileNumber = null,
            string department = null,
            string categoryName = null,
            string description = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, subject, companyName, plotNo, plotCategory, contactPersonName, emailId, mobileNumber, department, categoryName, description);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<FeedBack> ApplyFilter(
            IQueryable<FeedBack> query,
            string filterText,
            string subject = null,
            string companyName = null,
            string plotNo = null,
            string plotCategory = null,
            string contactPersonName = null,
            string emailId = null,
            string mobileNumber = null,
            string department = null,
            string categoryName = null,
            string description = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Subject.Contains(filterText) || e.CompanyName.Contains(filterText) || e.PlotNo.Contains(filterText) || e.PlotCategory.Contains(filterText) || e.ContactPersonName.Contains(filterText) || e.EmailId.Contains(filterText) || e.MobileNumber.Contains(filterText) || e.Department.Contains(filterText) || e.CategoryName.Contains(filterText) || e.Description.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(subject), e => e.Subject.Contains(subject))
                    .WhereIf(!string.IsNullOrWhiteSpace(companyName), e => e.CompanyName.Contains(companyName))
                    .WhereIf(!string.IsNullOrWhiteSpace(plotNo), e => e.PlotNo.Contains(plotNo))
                    .WhereIf(!string.IsNullOrWhiteSpace(plotCategory), e => e.PlotCategory.Contains(plotCategory))
                    .WhereIf(!string.IsNullOrWhiteSpace(contactPersonName), e => e.ContactPersonName.Contains(contactPersonName))
                    .WhereIf(!string.IsNullOrWhiteSpace(emailId), e => e.EmailId.Contains(emailId))
                    .WhereIf(!string.IsNullOrWhiteSpace(mobileNumber), e => e.MobileNumber.Contains(mobileNumber))
                    .WhereIf(!string.IsNullOrWhiteSpace(department), e => e.Department.Contains(department))
                    .WhereIf(!string.IsNullOrWhiteSpace(categoryName), e => e.CategoryName.Contains(categoryName))
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description));
        }
    }
}