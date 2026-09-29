using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.Commercials
{
    public partial interface ICommercialRepository : IRepository<Commercial, Guid>
    {
        Task<CommercialWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<CommercialWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string plotNo = null,
            string activityEn = null,
            string activityAr = null,
            string phone = null,
            string fax = null,
            string makaniNo = null,
            bool? isActive = null,
            int? orderMin = null,
            int? orderMax = null,
            Guid? subCategoryId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<CommercialWithNavigationProperties>> GetListWithPlotNoWithNavigationPropertiesAsync(
    string filterText = null,
    string titleEn = null,
    string titleAr = null,
    string plotNo = null,
    string activityEn = null,
    string activityAr = null,
    string phone = null,
    string fax = null,
    string makaniNo = null,
    bool? isActive = null,
    int? orderMin = null,
    int? orderMax = null,
    Guid? subCategoryId = null,
    string sorting = null,
    int maxResultCount = int.MaxValue,
    int skipCount = 0,
    CancellationToken cancellationToken = default
);

        Task<List<Commercial>> GetListAsync(
                    string filterText = null,
                    string titleEn = null,
                    string titleAr = null,
                    string plotNo = null,
                    string activityEn = null,
                    string activityAr = null,
                    string phone = null,
                    string fax = null,
                    string makaniNo = null,
                    bool? isActive = null,
                    int? orderMin = null,
                    int? orderMax = null,
                    string sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string plotNo = null,
            string activityEn = null,
            string activityAr = null,
            string phone = null,
            string fax = null,
            string makaniNo = null,
            bool? isActive = null,
            int? orderMin = null,
            int? orderMax = null,
            Guid? subCategoryId = null,
            CancellationToken cancellationToken = default);
    }
}