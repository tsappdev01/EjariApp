using DIP.Zones;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using DIP.ZoneParagraphs;

namespace DIP.ZoneParagraphs
{
    public class ZoneParagraphsDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IZoneParagraphRepository _zoneParagraphRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ZonesDataSeedContributor _zonesDataSeedContributor;

        public ZoneParagraphsDataSeedContributor(IZoneParagraphRepository zoneParagraphRepository, IUnitOfWorkManager unitOfWorkManager, ZonesDataSeedContributor zonesDataSeedContributor)
        {
            _zoneParagraphRepository = zoneParagraphRepository;
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

            await _zoneParagraphRepository.InsertAsync(new ZoneParagraph
            (
                id: Guid.Parse("a289ab56-f9c9-476d-aed3-4fb91d7d14be"),
                titleEn: "29824677252849f4834",
                titleAr: "2b3046adf5f64f13877166761200cfbbc88a252f5b0",
                subTilteEn: "1ba080c3e8c348cabd9e043ff92eb",
                subTitleAr: "79a4329ef7744eb080f314a92d11c656650709b3",
                descriptionEn: "bccd2c4d4e3746ab824d59e48921a30a",
                descriptionAr: "2e59cbe1521e4d2387f880c6ae2f5458ad",
                order: 658637134,
                isActive: true,
                zoneId: Guid.Parse("5968a2a9-f7fc-469f-904c-3f176982876e")
            ));

            await _zoneParagraphRepository.InsertAsync(new ZoneParagraph
            (
                id: Guid.Parse("eba7ff10-b3bd-495a-b338-0ab253e78f8a"),
                titleEn: "81746003b6894640861eb4e9ec5f6",
                titleAr: "c5401e500dc542599ef4c620b6c232e4498a0ac371cb49579aa87672ec42868620dcc7b3310f41c5a033",
                subTilteEn: "bbfbc4773490433889b77653d3feab6d3e086a8702b94edba0fe77bcd717fe770661f35a49544a70b27a53f773bc3",
                subTitleAr: "775051f946c04085a72802f6f535606f164c015d20924afba4f87b64667aea30",
                descriptionEn: "7a04fd10cf15",
                descriptionAr: "bfa3f3d85a874ece962921f7288ec5243f96072d7feb4076a8d536d1e536f23761f1455fc58d4c699820",
                order: 999731544,
                isActive: true,
                zoneId: Guid.Parse("5968a2a9-f7fc-469f-904c-3f176982876e")
            ));

            await _unitOfWorkManager.Current.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}