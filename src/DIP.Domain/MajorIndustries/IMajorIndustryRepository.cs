using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.MajorIndustries
{
    public interface IMajorIndustryRepository : IRepository<MajorIndustry, Guid>
    {
        Task<MajorIndustryWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<MajorIndustryWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string image = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? zoneId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<MajorIndustry>> GetListAsync(
                    string filterText = null,
                    string titleEn = null,
                    string titleAr = null,
                    string image = null,
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
            string image = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? zoneId = null,
            CancellationToken cancellationToken = default);
    }
}