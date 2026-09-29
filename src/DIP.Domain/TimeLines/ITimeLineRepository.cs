using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.TimeLines
{
    public interface ITimeLineRepository : IRepository<TimeLine, Guid>
    {
        Task<TimeLineWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<TimeLineWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string descriptionEn = null,
            string descriptionAr = null,
            string image = null,
            DateTime? timeLineDateMin = null,
            DateTime? timeLineDateMax = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? timeLineCategoryId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<TimeLine>> GetListAsync(
                    string filterText = null,
                    string titleEn = null,
                    string titleAr = null,
                    string descriptionEn = null,
                    string descriptionAr = null,
                    string image = null,
                    DateTime? timeLineDateMin = null,
                    DateTime? timeLineDateMax = null,
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
            string descriptionEn = null,
            string descriptionAr = null,
            string image = null,
            DateTime? timeLineDateMin = null,
            DateTime? timeLineDateMax = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? timeLineCategoryId = null,
            CancellationToken cancellationToken = default);
    }
}