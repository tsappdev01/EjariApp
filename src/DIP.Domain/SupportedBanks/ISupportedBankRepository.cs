using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.SupportedBanks
{
    public interface ISupportedBankRepository : IRepository<SupportedBank, Guid>
    {
        Task<List<SupportedBank>> GetListAsync(
            string filterText = null,
            string titleAr = null,
            string titleEn = null,
            bool? isActive = null,
            int? orderMin = null,
            int? orderMax = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filterText = null,
            string titleAr = null,
            string titleEn = null,
            bool? isActive = null,
            int? orderMin = null,
            int? orderMax = null,
            CancellationToken cancellationToken = default);
    }
}