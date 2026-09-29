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

namespace DIP.InquiryForms
{
    public class EfCoreInquiryFormRepository : EfCoreRepository<DIPDbContext, InquiryForm, Guid>, IInquiryFormRepository
    {
        public EfCoreInquiryFormRepository(IDbContextProvider<DIPDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<List<InquiryForm>> GetListAsync(
            string filterText = null,
            string companyName = null,
            string name = null,
            string email = null,
            string mobile = null,
            string fax = null,
            string phone = null,
            string inquiryType = null,
            string tradeLicensePlateOfIssue = null,
            string buyRent = null,
            double? spaceInSquareFeetMin = null,
            double? spaceInSquareFeetMax = null,
            string comments = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, companyName, name, email, mobile, fax, phone, inquiryType, tradeLicensePlateOfIssue, buyRent, spaceInSquareFeetMin, spaceInSquareFeetMax, comments);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? InquiryFormConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string companyName = null,
            string name = null,
            string email = null,
            string mobile = null,
            string fax = null,
            string phone = null,
            string inquiryType = null,
            string tradeLicensePlateOfIssue = null,
            string buyRent = null,
            double? spaceInSquareFeetMin = null,
            double? spaceInSquareFeetMax = null,
            string comments = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, companyName, name, email, mobile, fax, phone, inquiryType, tradeLicensePlateOfIssue, buyRent, spaceInSquareFeetMin, spaceInSquareFeetMax, comments);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<InquiryForm> ApplyFilter(
            IQueryable<InquiryForm> query,
            string filterText,
            string companyName = null,
            string name = null,
            string email = null,
            string mobile = null,
            string fax = null,
            string phone = null,
            string inquiryType = null,
            string tradeLicensePlateOfIssue = null,
            string buyRent = null,
            double? spaceInSquareFeetMin = null,
            double? spaceInSquareFeetMax = null,
            string comments = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.CompanyName.Contains(filterText) || e.Name.Contains(filterText) || e.Email.Contains(filterText) || e.Mobile.Contains(filterText) || e.Fax.Contains(filterText) || e.Phone.Contains(filterText) || e.InquiryType.Contains(filterText) || e.TradeLicensePlateOfIssue.Contains(filterText) || e.BuyRent.Contains(filterText) || e.Comments.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(companyName), e => e.CompanyName.Contains(companyName))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                    .WhereIf(!string.IsNullOrWhiteSpace(email), e => e.Email.Contains(email))
                    .WhereIf(!string.IsNullOrWhiteSpace(mobile), e => e.Mobile.Contains(mobile))
                    .WhereIf(!string.IsNullOrWhiteSpace(fax), e => e.Fax.Contains(fax))
                    .WhereIf(!string.IsNullOrWhiteSpace(phone), e => e.Phone.Contains(phone))
                    .WhereIf(!string.IsNullOrWhiteSpace(inquiryType), e => e.InquiryType.Contains(inquiryType))
                    .WhereIf(!string.IsNullOrWhiteSpace(tradeLicensePlateOfIssue), e => e.TradeLicensePlateOfIssue.Contains(tradeLicensePlateOfIssue))
                    .WhereIf(!string.IsNullOrWhiteSpace(buyRent), e => e.BuyRent.Contains(buyRent))
                    .WhereIf(spaceInSquareFeetMin.HasValue, e => e.SpaceInSquareFeet >= spaceInSquareFeetMin.Value)
                    .WhereIf(spaceInSquareFeetMax.HasValue, e => e.SpaceInSquareFeet <= spaceInSquareFeetMax.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(comments), e => e.Comments.Contains(comments));
        }
    }
}