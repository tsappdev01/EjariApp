using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using DIP.Zones;

namespace DIP.Zones
{
    public class ZonesDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IZoneRepository _zoneRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ZonesDataSeedContributor(IZoneRepository zoneRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _zoneRepository = zoneRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _zoneRepository.InsertAsync(new Zone
            (
                id: Guid.Parse("cd8acf73-c72b-479b-9c4b-ccc9db024924"),
                titleEn: "c83cf5991a744e219",
                titleAR: "fa81f0ceff164520be951daa8003b06110c39f447b824312904e3391e91caa4f3322631993364ef282920e90",
                metaTitleEn: "d1422938797147129c8ad5bc2d58883fb3",
                metaDescriptionEn: "a08b59cd08e14d2",
                metaTitleAr: "286517c1cedb4f6091acb65e8425bfd33a91af097cf7485d8827ef0e9afb3a8a52da96ccef824ddeb",
                metaDescriptionAr: "9d9c4b7dde45472ab2fff31db3542d745a2c3edcbdf8465fb76ea0ead832327ba29ce23746154679ab0652b12659230c56",
                slug: "a8ba692517b34f0a",
                summaryEn: "716a1cd2470c4fb493d04ac63b8553eb3801",
                summaryAr: "b666fe84a583466fb23a9adede944b6d5dca82fe436540c0a21e",
                image: "075dae425e0d4873a294ebb6c2612ab65a8b0bd96f064ce29e553c709d44ca0143c013f53cf14d2c9cb15f535497ac431",
                headerImage: "bc85942a",
                order: 1828754003,
                isFeature: true,
                isActive: true
            ));

            await _zoneRepository.InsertAsync(new Zone
            (
                id: Guid.Parse("0216533c-0a82-4107-b1d5-1abcfcedd1db"),
                titleEn: "3a8e8f5c7511",
                titleAR: "470bf2a284b141f9bcb79d2e854df8915b25f6a65d37417eb3b53497a0742c8a6dcdcf7",
                metaTitleEn: "66f15728bd74400ea4ab8b95250d96d4febf804765cc4c6780311645184163fdbd1016880b684b1",
                metaDescriptionEn: "8eeb8a2a5d584a1cbb7ea6fb1fe450cbe1d307825cac445d",
                metaTitleAr: "f599e223f3934692ae",
                metaDescriptionAr: "1105e395f8",
                slug: "7cf7d5e1215d4af4825bf09ab3ea44eddae875a6838143ca96f4929a42",
                summaryEn: "c28ecbad0c0c4047ae6c0c89ca996e4c73ca54",
                summaryAr: "eaa2a39f0a0246ac915387fe3bd59834ad020140b9d34040a4",
                image: "c71e8965d9684d0e897e",
                headerImage: "66c33e6804994e059a6f7aa5c90879482d919bf2d91b4fffa242",
                order: 423429694,
                isFeature: true,
                isActive: true
            ));

            await _unitOfWorkManager.Current.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}