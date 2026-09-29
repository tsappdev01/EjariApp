using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.InquiryForms
{
    public interface IInquiryFormRepository : IRepository<InquiryForm, Guid>
    {
        Task<List<InquiryForm>> GetListAsync(
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
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
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
            CancellationToken cancellationToken = default);
    }
}