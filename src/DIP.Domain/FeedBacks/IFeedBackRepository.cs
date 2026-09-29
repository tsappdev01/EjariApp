using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.FeedBacks
{
    public interface IFeedBackRepository : IRepository<FeedBack, Guid>
    {
        Task<List<FeedBack>> GetListAsync(
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
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
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
            CancellationToken cancellationToken = default);
    }
}