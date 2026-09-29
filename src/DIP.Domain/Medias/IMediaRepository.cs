using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.Medias
{
    public interface IMediaRepository : IRepository<Media, Guid>
    {
        Task<MediaWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<MediaWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? zoneParagraphId = null,
            Guid? amenityParagraphId = null,
            Guid? mediaGalleryId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<Media>> GetListAsync(
                    string filterText = null,
                    string titleEn = null,
                    string titleAr = null,
                    string file = null,
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
            string file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? zoneParagraphId = null,
            Guid? amenityParagraphId = null,
            Guid? mediaGalleryId = null,
            CancellationToken cancellationToken = default);
    }
}