using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.SubCategories
{
    public class SubCategoryManager : DomainService
    {
        private readonly ISubCategoryRepository _subCategoryRepository;

        public SubCategoryManager(ISubCategoryRepository subCategoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;
        }

        public async Task<SubCategory> CreateAsync(
        Guid categoryId, string titleAr, string titleEn, int order, bool isFeature, bool isActive)
        {
            Check.NotNull(categoryId, nameof(categoryId));
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));

            var subCategory = new SubCategory(
             GuidGenerator.Create(),
             categoryId, titleAr, titleEn, order, isFeature, isActive
             );

            return await _subCategoryRepository.InsertAsync(subCategory);
        }

        public async Task<SubCategory> UpdateAsync(
            Guid id,
            Guid categoryId, string titleAr, string titleEn, int order, bool isFeature, bool isActive, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNull(categoryId, nameof(categoryId));
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));

            var subCategory = await _subCategoryRepository.GetAsync(id);

            subCategory.CategoryId = categoryId;
            subCategory.TitleAr = titleAr;
            subCategory.TitleEn = titleEn;
            subCategory.Order = order;
            subCategory.IsFeature = isFeature;
            subCategory.IsActive = isActive;

            subCategory.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _subCategoryRepository.UpdateAsync(subCategory);
        }

    }
}