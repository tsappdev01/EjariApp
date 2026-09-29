using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.Zones
{
    public partial interface IZoneRepository : IRepository<Zone, Guid>
    {
        Task<List<Zone>> GetListAsync(
            string filterText = null,
            string titleEn = null,
            string titleAR = null,
            string metaTitleEn = null,
            string metaDescriptionEn = null,
            string metaTitleAr = null,
            string metaDescriptionAr = null,
            string slug = null,
            string summaryEn = null,
            string summaryAr = null,
            string image = null,
            string headerImage = null,
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
            string titleEn = null,
            string titleAR = null,
            string metaTitleEn = null,
            string metaDescriptionEn = null,
            string metaTitleAr = null,
            string metaDescriptionAr = null,
            string slug = null,
            string summaryEn = null,
            string summaryAr = null,
            string image = null,
            string headerImage = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isFeature = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default);
    }
}