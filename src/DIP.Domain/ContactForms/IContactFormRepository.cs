using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.ContactForms
{
    public interface IContactFormRepository : IRepository<ContactForm, Guid>
    {
        Task<List<ContactForm>> GetListAsync(
            string filterText = null,
            string fullName = null,
            string email = null,
            string subject = null,
            string message = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filterText = null,
            string fullName = null,
            string email = null,
            string subject = null,
            string message = null,
            CancellationToken cancellationToken = default);
    }
}