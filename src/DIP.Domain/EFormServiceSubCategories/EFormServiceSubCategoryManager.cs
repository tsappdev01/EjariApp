using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.EFormServiceSubCategories
{
    public class EFormServiceSubCategoryManager : DomainService
    {
        private readonly IEFormServiceSubCategoryRepository _eFormServiceSubCategoryRepository;

        public EFormServiceSubCategoryManager(IEFormServiceSubCategoryRepository eFormServiceSubCategoryRepository)
        {
            _eFormServiceSubCategoryRepository = eFormServiceSubCategoryRepository;
        }

        public async Task<EFormServiceSubCategory> CreateAsync(
        Guid eFormServiceId, string titleEn, string titleAr, string file, int order, bool isActive)
        {
            Check.NotNull(eFormServiceId, nameof(eFormServiceId));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));

            var eFormServiceSubCategory = new EFormServiceSubCategory(
             GuidGenerator.Create(),
             eFormServiceId, titleEn, titleAr, file, order, isActive
             );

            return await _eFormServiceSubCategoryRepository.InsertAsync(eFormServiceSubCategory);
        }

        public async Task<EFormServiceSubCategory> UpdateAsync(
            Guid id,
            Guid eFormServiceId, string titleEn, string titleAr, string file, int order, bool isActive, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNull(eFormServiceId, nameof(eFormServiceId));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));

            var eFormServiceSubCategory = await _eFormServiceSubCategoryRepository.GetAsync(id);

            eFormServiceSubCategory.EFormServiceId = eFormServiceId;
            eFormServiceSubCategory.TitleEn = titleEn;
            eFormServiceSubCategory.TitleAr = titleAr;
            eFormServiceSubCategory.File = file;
            eFormServiceSubCategory.Order = order;
            eFormServiceSubCategory.IsActive = isActive;

            eFormServiceSubCategory.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _eFormServiceSubCategoryRepository.UpdateAsync(eFormServiceSubCategory);
        }

    }
}