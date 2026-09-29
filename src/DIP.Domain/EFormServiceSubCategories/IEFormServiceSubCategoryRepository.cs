using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.EFormServiceSubCategories
{
    public interface IEFormServiceSubCategoryRepository : IRepository<EFormServiceSubCategory, Guid>
    {
        Task<EFormServiceSubCategoryWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<EFormServiceSubCategoryWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? eFormServiceId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<EFormServiceSubCategory>> GetListAsync(
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
            Guid? eFormServiceId = null,
            CancellationToken cancellationToken = default);
    }
}