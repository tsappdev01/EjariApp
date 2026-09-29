using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.DipBranches
{
    public interface IDipBranchRepository : IRepository<DipBranch, Guid>
    {
        Task<List<DipBranch>> GetListAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string subTitleEn = null,
            string subTitleAr = null,
            string phone = null,
            string alternativePhone = null,
            string email = null,
            string alternativeEmail = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string subTitleEn = null,
            string subTitleAr = null,
            string phone = null,
            string alternativePhone = null,
            string email = null,
            string alternativeEmail = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default);
    }
}