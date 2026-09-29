using DIP.EFormServiceSubCategories;
using DIP.Zones;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.EFormServices
{
    public partial interface IEFormServiceRepository 
    {
        Task<List<EFromServiceWithDetails>> GetListWithDetailsAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
            );
    }
}