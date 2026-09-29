using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using DIP.Categories;

namespace DIP.Categories
{
    public class CategoriesDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public CategoriesDataSeedContributor(ICategoryRepository categoryRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _categoryRepository = categoryRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _categoryRepository.InsertAsync(new Category
            (
                id: Guid.Parse("e0b69f5f-79f3-4361-b020-8736ac72984c"),
                titleAr: "1f12efc03f0c4f06af1718ec50935ee8048b136309314d8bbe42bfdf17e691e349075cfa7b584edd8079d6a12d8797c6",
                titleEn: "6e4d4b10c87a47aeb1357a2aa7d4f5851ab60dadb3724ac",
                order: 1323588127,
                isFeature: false,
                isActive: true
            ));

            await _categoryRepository.InsertAsync(new Category
            (
                id: Guid.Parse("d199f211-8c24-409c-8a60-bc272b1ad3a3"),
                titleAr: "46f07a97752f487da48cf3c770316b236d6e278ec178411d920cfbb30488c5a8ba09",
                titleEn: "717baa57f77f40388f95b3673ccfecf4304c9ba6711e49feb31a8ce2328bf635eb45b97",
                order: 406539884,
                 isFeature: false,
                isActive: true
            ));

            await _unitOfWorkManager.Current.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}