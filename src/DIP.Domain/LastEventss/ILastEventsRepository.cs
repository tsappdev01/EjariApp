using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.LastEventss
{
    public partial interface ILastEventsRepository : IRepository<LastEvents, Guid>
    {
        Task<List<LastEvents>> GetListAsync(
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
            DateTime? startDateMin = null,
            DateTime? startDateMax = null,
            DateTime? endDateMin = null,
            DateTime? endDateMax = null,
            bool? isActive = null,
            string locationAr = null,
            string locationEn = null,
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
            DateTime? startDateMin = null,
            DateTime? startDateMax = null,
            DateTime? endDateMin = null,
            DateTime? endDateMax = null,
            bool? isActive = null,
            string locationAr = null,
            string locationEn = null,
            CancellationToken cancellationToken = default);
    }
}