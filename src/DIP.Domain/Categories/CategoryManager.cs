using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.Categories
{
    public class CategoryManager : DomainService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryManager(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Category> CreateAsync(
        string titleAr, string titleEn, int order, bool isFeature, bool isActive)
        {
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));

            var category = new Category(
             GuidGenerator.Create(),
             titleAr, titleEn, order, isFeature, isActive
             );

            return await _categoryRepository.InsertAsync(category);
        }

        public async Task<Category> UpdateAsync(
            Guid id,
            string titleAr, string titleEn, int order, bool isFeature, bool isActive, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));

            var category = await _categoryRepository.GetAsync(id);

            category.TitleAr = titleAr;
            category.TitleEn = titleEn;
            category.Order = order;
            category.IsFeature = isFeature;
            category.IsActive = isActive;

            category.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _categoryRepository.UpdateAsync(category);
        }

    }
}