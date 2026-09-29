using DIP.Categories;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using DIP.SubCategories;

namespace DIP.SubCategories
{
    public class SubCategoriesDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly CategoriesDataSeedContributor _categoriesDataSeedContributor;

        public SubCategoriesDataSeedContributor(ISubCategoryRepository subCategoryRepository, IUnitOfWorkManager unitOfWorkManager, CategoriesDataSeedContributor categoriesDataSeedContributor)
        {
            _subCategoryRepository = subCategoryRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _categoriesDataSeedContributor = categoriesDataSeedContributor;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _categoriesDataSeedContributor.SeedAsync(context);

            await _subCategoryRepository.InsertAsync(new SubCategory
            (
                id: Guid.Parse("8d83126a-a8e9-4b30-99a4-bef92dfd31a7"),
                titleAr: "ce1fade4bd8c46d68557700aefc1a94c6e262373e36a4059b059d",
                titleEn: "4500fb4b40a6407ca9de8722754c6b8fa200658252bb45729e364f7196e79602329d45622f7d497",
                order: 1111603910,
                isFeature: false,
                isActive: true,
                categoryId: Guid.Parse("059ae14e-2021-4827-b59e-f648a17a3c0d")
            ));

            await _subCategoryRepository.InsertAsync(new SubCategory
            (
                id: Guid.Parse("19c4fe77-b7f7-4426-92ab-28c56a19c9c5"),
                titleAr: "16f6e6112a57484683d47dd4",
                titleEn: "da117cf50ff04479bb3f8da7a6d3eb1191fb3efef010454ab6caddade45ba3a9b1b25e4",
                order: 1240238446,
                isFeature: false,
                isActive: true,
                categoryId: Guid.Parse("059ae14e-2021-4827-b59e-f648a17a3c0d")
            ));

            await _unitOfWorkManager.Current.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}