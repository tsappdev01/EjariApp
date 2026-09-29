using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.PressReleases
{
    public partial interface IPressReleaseRepository : IRepository<PressRelease, Guid>
    {
        Task<List<PressRelease>> GetListAsync(
            string filterText = null,
            string titleAr = null,
            string titleEn = null,
            string metaTitleAr = null,
            string metaTitleEn = null,
            string metaDescriptionEn = null,
            string metaDescriptionAr = null,
            bool? isFeatured = null,
            string slug = null,
            string image = null,
            string headerImage = null,
            string descriptionAr = null,
            string descriptionEn = null,
            string summaryEn = null,
            string summaryAr = null,
            int? orderMin = null,
            int? orderMax = null,
            DateTime? dateMin = null,
            DateTime? dateMax = null,
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
            string metaTitleAr = null,
            string metaTitleEn = null,
            string metaDescriptionEn = null,
            string metaDescriptionAr = null,
            bool? isFeatured = null,
            string slug = null,
            string image = null,
            string headerImage = null,
            string descriptionAr = null,
            string descriptionEn = null,
            string summaryEn = null,
            string summaryAr = null,
            int? orderMin = null,
            int? orderMax = null,
            DateTime? dateMin = null,
            DateTime? dateMax = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default);
    }
}