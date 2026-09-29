using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.Commercials
{
    public partial interface ICommercialRepository
    {

        Task<List<CommercialWithDetails>> GetListWithDetailsAsync(
              string filterText = null,
              string titleEn = null,
              string titleAr = null,
              string plotNo = null,
              string activityEn = null,
              string activityAr = null,
              string phone = null,
              string fax = null,
              bool? isActive = null,
              int? orderMin = null,
              int? orderMax = null,
              List<Guid> subCategoryIds = null,
              char? startWithLetter = null,
              string sorting = null,
              int maxResultCount = int.MaxValue,
              int skipCount = 0,
              CancellationToken cancellationToken = default);

        Task<long> GetCountDetailsAsync(
        string filterText = null,
        string titleEn = null,
        string titleAr = null,
        string plotNo = null,
        string activityEn = null,
        string activityAr = null,
        string phone = null,
        string fax = null,
        bool? isActive = null,
        int? orderMin = null,
        int? orderMax = null,
        List<Guid> subCategoryIds = null,
        char? startWithLetter = null,
        CancellationToken cancellationToken = default);

        Task<List<CommercialWithDetails>> GetListWithDetailsByTextAsync(
      string filterText = null,
      string sorting = null,
      int maxResultCount = int.MaxValue,
      int skipCount = 0,
      CancellationToken cancellationToken = default);

        Task<long> GetCountDetailsByTextAsync(
string filterText = null,
CancellationToken cancellationToken = default);

    }
}