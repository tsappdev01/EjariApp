using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.AmenityParagraphs
{
    public interface IAmenityParagraphRepository : IRepository<AmenityParagraph, Guid>
    {
        Task<AmenityParagraphWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<AmenityParagraphWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string subTitleEn = null,
            string subTitleAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string buttonUrl = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? amenityId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<AmenityParagraph>> GetListAsync(
                    string filterText = null,
                    string titleEn = null,
                    string titleAr = null,
                    string subTitleEn = null,
                    string subTitleAr = null,
                    string descriptionEn = null,
                    string descriptionAr = null,
                    string buttonUrl = null,
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
            string descriptionEn = null,
            string descriptionAr = null,
            string buttonUrl = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? amenityId = null,
            CancellationToken cancellationToken = default);
    }
}