using DIP.Zones;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using DIP.MajorIndustries;

namespace DIP.MajorIndustries
{
    public class MajorIndustriesDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IMajorIndustryRepository _majorIndustryRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ZonesDataSeedContributor _zonesDataSeedContributor;

        public MajorIndustriesDataSeedContributor(IMajorIndustryRepository majorIndustryRepository, IUnitOfWorkManager unitOfWorkManager, ZonesDataSeedContributor zonesDataSeedContributor)
        {
            _majorIndustryRepository = majorIndustryRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _zonesDataSeedContributor = zonesDataSeedContributor;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _zonesDataSeedContributor.SeedAsync(context);

            await _majorIndustryRepository.InsertAsync(new MajorIndustry
            (
                id: Guid.Parse("91857fcf-ca93-49ad-99b2-6614beb9f27b"),
                titleEn: "824d12d3626641d38c10d74a39e520ef63d7f55286ae48c59bab84f4d9eeb84ec2f2ffd7f9be4e0f85082ce709d4f4ca",
                titleAr: "d0994343dc8b4cf8b086d6d79bb5e5e00dc00ff642544",
                image: "0c1c663358a244bbb1f970efcf8b0eee8ade8e4bb23647baaed39843d",
                order: 1769846843,
                isActive: true,
                zoneId: Guid.Parse("5968a2a9-f7fc-469f-904c-3f176982876e")
            ));

            await _majorIndustryRepository.InsertAsync(new MajorIndustry
            (
                id: Guid.Parse("c85e452f-64fa-4b8a-bc92-36e72829b3ca"),
                titleEn: "22e2197f534b476eb0b211a3c45465c51fefc88569bb4ab2bdb390b8da42ea4d5162527252",
                titleAr: "eec52b1fa370462fbe82c28417a7e2be5999de3031404b0a81dc4f4e3fd25523dab754f3",
                image: "431fc7f21fdc4876b8d46425ef9de5512b693f054cfd40b291",
                order: 1872819204,
                isActive: true,
                zoneId: Guid.Parse("5968a2a9-f7fc-469f-904c-3f176982876e")
            ));

            await _unitOfWorkManager.Current.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}