using DIP.EFormServices;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using DIP.EFormServiceSubCategories;

namespace DIP.EFormServiceSubCategories
{
    public class EFormServiceSubCategoriesDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IEFormServiceSubCategoryRepository _eFormServiceSubCategoryRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly EFormServicesDataSeedContributor _eFormServicesDataSeedContributor;

        public EFormServiceSubCategoriesDataSeedContributor(IEFormServiceSubCategoryRepository eFormServiceSubCategoryRepository, IUnitOfWorkManager unitOfWorkManager, EFormServicesDataSeedContributor eFormServicesDataSeedContributor)
        {
            _eFormServiceSubCategoryRepository = eFormServiceSubCategoryRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _eFormServicesDataSeedContributor = eFormServicesDataSeedContributor;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _eFormServicesDataSeedContributor.SeedAsync(context);

            await _eFormServiceSubCategoryRepository.InsertAsync(new EFormServiceSubCategory
            (
                id: Guid.Parse("e5dac3a2-de9e-441b-b2c0-e95491e335ba"),
                titleEn: "fc1b011eb07847b3bf47f7ad902e",
                titleAr: "4d1eddb802494fc9bcbf21a834e81dcca1010f95d6ed49718425feb7e311d6691591dcfe92ce4bf9ab7906a688365830c33",
                file: "9893e9ace63a4db780fc6125ab15d053632d91f01e0f42979",
                order: 1937293708,
                isActive: true,
                eFormServiceId: Guid.Parse("f3a4fb45-a8e8-4d7b-9e5f-53cca28817ce")
            ));

            await _eFormServiceSubCategoryRepository.InsertAsync(new EFormServiceSubCategory
            (
                id: Guid.Parse("857e2f65-1d71-42e5-99b9-52fc4255bfee"),
                titleEn: "ae01a134f3344e3ea3e109e5aa98c077fc238f8c85af4868b63696b940f33b9174e93e6944744a",
                titleAr: "b999a72c00be465",
                file: "6ac9b5c7c7ac44c8b",
                order: 1893382200,
                isActive: true,
                eFormServiceId: Guid.Parse("f3a4fb45-a8e8-4d7b-9e5f-53cca28817ce")
            ));

            await _unitOfWorkManager.Current.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}