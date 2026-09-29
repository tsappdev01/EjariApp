using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.Commercials
{
    public class CommercialManager : DomainService
    {
        private readonly ICommercialRepository _commercialRepository;

        public CommercialManager(ICommercialRepository commercialRepository)
        {
            _commercialRepository = commercialRepository;
        }

        public async Task<Commercial> CreateAsync(
        Guid subCategoryId, string titleEn, string titleAr, string plotNo, string activityEn, string activityAr, string phone, string fax, string makaniNo, bool isActive, int order)
        {
            Check.NotNull(subCategoryId, nameof(subCategoryId));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));

            var commercial = new Commercial(
             GuidGenerator.Create(),
             subCategoryId, titleEn, titleAr, plotNo, activityEn, activityAr, phone, fax, makaniNo, isActive, order
             );

            return await _commercialRepository.InsertAsync(commercial);
        }

        public async Task<Commercial> UpdateAsync(
            Guid id,
            Guid subCategoryId, string titleEn, string titleAr, string plotNo, string activityEn, string activityAr, string phone, string fax, string makaniNo, bool isActive, int order, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNull(subCategoryId, nameof(subCategoryId));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));

            var commercial = await _commercialRepository.GetAsync(id);

            commercial.SubCategoryId = subCategoryId;
            commercial.TitleEn = titleEn;
            commercial.TitleAr = titleAr;
            commercial.PlotNo = plotNo;
            commercial.ActivityEn = activityEn;
            commercial.ActivityAr = activityAr;
            commercial.Phone = phone;
            commercial.Fax = fax;
            commercial.MakaniNo = makaniNo;
            commercial.IsActive = isActive;
            commercial.Order = order;

            commercial.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _commercialRepository.UpdateAsync(commercial);
        }

        public async Task<Commercial> UpdateMakaniAsync(
    Guid id, string makaniNo
)
        {

            var commercial = await _commercialRepository.GetAsync(id);
            commercial.MakaniNo = makaniNo;
            return await _commercialRepository.UpdateAsync(commercial);
        }

    }
}