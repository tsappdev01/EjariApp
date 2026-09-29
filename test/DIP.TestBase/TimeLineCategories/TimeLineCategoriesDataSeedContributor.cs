using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using DIP.TimeLineCategories;

namespace DIP.TimeLineCategories
{
    public class TimeLineCategoriesDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly ITimeLineCategoryRepository _timeLineCategoryRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public TimeLineCategoriesDataSeedContributor(ITimeLineCategoryRepository timeLineCategoryRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _timeLineCategoryRepository = timeLineCategoryRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _timeLineCategoryRepository.InsertAsync(new TimeLineCategory
            (
                id: Guid.Parse("b5a456d9-416a-4464-a807-1dc3f46c7440"),
                titleEn: "15abe51edc0742d2af7fabbcc6c9ecd8284758f9369d4ab0b84b",
                titleAr: "235291c54c7947d1aca1656275ed40141a31518bf0204b439",
                order: 2135932360,
                isActive: true
            ));

            await _timeLineCategoryRepository.InsertAsync(new TimeLineCategory
            (
                id: Guid.Parse("e86fe1ad-fc20-405d-b37a-8ccf64d26436"),
                titleEn: "f614097e5db045e2af",
                titleAr: "9291e49819124c17bd86f3e3336e572499554",
                order: 406316497,
                isActive: true
            ));

            await _unitOfWorkManager.Current.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}