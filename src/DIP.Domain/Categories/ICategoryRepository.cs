using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.Categories
{
    public interface ICategoryRepository : IRepository<Category, Guid>
    {
        Task<List<Category>> GetListAsync(
            string filterText = null,
            string titleAr = null,
            string titleEn = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isFeature = null,
            bool? isActive = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filterText = null,
            string titleAr = null,
            string titleEn = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isFeature = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default);
    }
}