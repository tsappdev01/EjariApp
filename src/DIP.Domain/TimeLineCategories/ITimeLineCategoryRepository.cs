using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.TimeLineCategories
{
    public partial interface ITimeLineCategoryRepository : IRepository<TimeLineCategory, Guid>
    {
        Task<List<TimeLineCategory>> GetListAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
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
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default);
    }
}