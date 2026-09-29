using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.EServices
{
    public interface IEServiceRepository : IRepository<EService, Guid>
    {
        Task<List<EService>> GetListAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string slug = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string image = null,
            string headerImage = null,
            string metaTitleEn = null,
            string metaTitleAr = null,
            string metaDescriptionEn = null,
            string metaDescriptionAr = null,
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
            string slug = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string image = null,
            string headerImage = null,
            string metaTitleEn = null,
            string metaTitleAr = null,
            string metaDescriptionEn = null,
            string metaDescriptionAr = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default);
    }
}