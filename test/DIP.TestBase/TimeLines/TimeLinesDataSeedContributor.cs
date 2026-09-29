using DIP.TimeLineCategories;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using DIP.TimeLines;

namespace DIP.TimeLines
{
    public class TimeLinesDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly ITimeLineRepository _timeLineRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly TimeLineCategoriesDataSeedContributor _timeLineCategoriesDataSeedContributor;

        public TimeLinesDataSeedContributor(ITimeLineRepository timeLineRepository, IUnitOfWorkManager unitOfWorkManager, TimeLineCategoriesDataSeedContributor timeLineCategoriesDataSeedContributor)
        {
            _timeLineRepository = timeLineRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _timeLineCategoriesDataSeedContributor = timeLineCategoriesDataSeedContributor;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _timeLineCategoriesDataSeedContributor.SeedAsync(context);

            await _timeLineRepository.InsertAsync(new TimeLine
            (
                id: Guid.Parse("b1787d2c-f9c4-4fdc-8fa0-869bb532ab9b"),
                titleEn: "4340b3ad5",
                titleAr: "e3fb30a74e724d699",
                descriptionEn: "d0bd60",
                descriptionAr: "ea0544ded94349e1be115af202428c32861f289c45254954bbec1",
                image: "cf0943ee90e04b719fe6524f9ea24abb613295850ccd416dbe260de9c241eaae51eecebf6a094cd8a246e",
                timeLineDate: new DateTime(2021, 3, 20),
                order: 958950630,
                isActive: true,
                timeLineCategoryId: Guid.Parse("b5a456d9-416a-4464-a807-1dc3f46c7440")
            ));

            await _timeLineRepository.InsertAsync(new TimeLine
            (
                id: Guid.Parse("34fb95d6-a232-4878-8f04-95a0a3d339ac"),
                titleEn: "69bed59410e54c5baf85fcf2d8bd",
                titleAr: "23479034a5144122b3b23e3a890c51cbec05b1be933a477186ac96358224a4e7b23b6f43b8684ebd98b293",
                descriptionEn: "a78abfe977a941378",
                descriptionAr: "e63f6bbd8e3b4958a99f644dba20bbd28",
                image: "b5c5ca34dd8647c79f358145f72fd1285b",
                timeLineDate: new DateTime(2000, 10, 17),
                order: 1051389669,
                isActive: true,
                timeLineCategoryId: Guid.Parse("b5a456d9-416a-4464-a807-1dc3f46c7440")
            ));

            await _unitOfWorkManager.Current.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}