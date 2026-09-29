using DIP.Amenities;
using DIP.MajorIndustries;
using DIP.ZoneParagraphs;
using DIP.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace DIP.EFormServices
{
    public partial class EfCoreEFormServiceRepository
    {
        public async Task<List<EFromServiceWithDetails>> GetListWithDetailsAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return (await GetDbSetAsync())
                     .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.TitleEn.Contains(filterText) || e.TitleAr.Contains(filterText))
                     .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.TitleEn.Contains(titleEn))
                     .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.TitleAr.Contains(titleAr))
                     .WhereIf(orderMin.HasValue, e => e.Order >= orderMin.Value)
                     .WhereIf(orderMax.HasValue, e => e.Order <= orderMax.Value)
                     .WhereIf(isActive.HasValue, e => e.IsActive == isActive)
                     .OrderBy(string.IsNullOrWhiteSpace(sorting) ? EFormServiceConsts.GetDefaultSorting(false) : sorting)
                    .Select(eFormService => new EFromServiceWithDetails
                    {
                        EFormService = eFormService,
                        EFormServiceSubCategories = dbContext.EFormServiceSubCategories.Where(e => e.IsActive && e.EFormServiceId == eFormService.Id)
                                            .OrderBy(a => a.Order).ToList()
                    }).ToList();
        }
    }
}