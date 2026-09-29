using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.ZoneParagraphs
{
    public interface IZoneParagraphRepository : IRepository<ZoneParagraph, Guid>
    {
        Task<ZoneParagraphWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<ZoneParagraphWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string subTilteEn = null,
            string subTitleAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? zoneId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<ZoneParagraph>> GetListAsync(
                    string filterText = null,
                    string titleEn = null,
                    string titleAr = null,
                    string subTilteEn = null,
                    string subTitleAr = null,
                    string descriptionEn = null,
                    string descriptionAr = null,
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
            string subTilteEn = null,
            string subTitleAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? zoneId = null,
            CancellationToken cancellationToken = default);
    }
}