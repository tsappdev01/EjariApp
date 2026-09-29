using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.PageInfoSections
{
    public interface IPageInfoSectionRepository : IRepository<PageInfoSection, Guid>
    {
        Task<PageInfoSectionWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<PageInfoSectionWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string subTitleEn = null,
            string subTitleAr = null,
            string summaryEn = null,
            string summaryAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string pageSectionMedia = null,
            string youtubeUrl = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? pageInfoId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<PageInfoSection>> GetListAsync(
                    string filterText = null,
                    string titleEn = null,
                    string titleAr = null,
                    string subTitleEn = null,
                    string subTitleAr = null,
                    string summaryEn = null,
                    string summaryAr = null,
                    string descriptionEn = null,
                    string descriptionAr = null,
                    string pageSectionMedia = null,
                    string youtubeUrl = null,
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
            string summaryEn = null,
            string summaryAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string pageSectionMedia = null,
            string youtubeUrl = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? pageInfoId = null,
            CancellationToken cancellationToken = default);
    }
}