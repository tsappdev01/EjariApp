using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.MediaGalleries
{
    public partial interface IMediaGalleryRepository : IRepository<MediaGallery, Guid>
    {
        Task<List<MediaGallery>> GetListAsync(
            string filterText = null,
            string titleEn = null,
            string slug = null,
            string titleAr = null,
            string metaTitleEn = null,
            string metaTitleAr = null,
            string metaDescriptionEn = null,
            string metaDescriptionAr = null,
            string summaryEn = null,
            string summaryAr = null,
            string headerImage = null,
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
            string slug = null,
            string titleAr = null,
            string metaTitleEn = null,
            string metaTitleAr = null,
            string metaDescriptionEn = null,
            string metaDescriptionAr = null,
            string summaryEn = null,
            string summaryAr = null,
            string headerImage = null,
            string image = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default);
    }
}