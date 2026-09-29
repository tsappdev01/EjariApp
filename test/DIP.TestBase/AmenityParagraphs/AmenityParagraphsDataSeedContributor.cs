using DIP.Amenities;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using DIP.AmenityParagraphs;

namespace DIP.AmenityParagraphs
{
    public class AmenityParagraphsDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IAmenityParagraphRepository _amenityParagraphRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly AmenitiesDataSeedContributor _amenitiesDataSeedContributor;

        public AmenityParagraphsDataSeedContributor(IAmenityParagraphRepository amenityParagraphRepository, IUnitOfWorkManager unitOfWorkManager, AmenitiesDataSeedContributor amenitiesDataSeedContributor)
        {
            _amenityParagraphRepository = amenityParagraphRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _amenitiesDataSeedContributor = amenitiesDataSeedContributor;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _amenitiesDataSeedContributor.SeedAsync(context);

            await _amenityParagraphRepository.InsertAsync(new AmenityParagraph
            (
                id: Guid.Parse("e7f94fcc-8906-4dad-a3ce-045e04bc2bf4"),
                titleEn: "b0916c1c5795471194b2bc2fd300164f093951ac2d92459c811f56fb04ccbffba63ac95b",
                titleAr: "c4416c6fdb6e4be",
                subTitleEn: "5324b02eea04437c9cf6c903a3cba933ef829aa5ad754ca9abc7df98f1f54da19d85",
                subTitleAr: "ed3ef49d96a0493e9072f7dc46521800fbbcd8a5c4c54405b1714a9e119ee7087888bf15f7ed41f5af0e94bcf2685f46",
                descriptionEn: "c3caa01349f44d598438e92a44dbac6aab68f1856f7a4002aa7df76d8845585c38a0546e883",
                descriptionAr: "d32b353822",
                buttonUrl: "ded2c34cbea447369a6c88f6a939382c304cfd21b4",
                order: 1949323810,
                isActive: true,
                amenityId: Guid.Parse("d2da12fa-8dc8-4585-a9b2-1e074fde2f9b")
            ));

            await _amenityParagraphRepository.InsertAsync(new AmenityParagraph
            (
                id: Guid.Parse("af060edf-39ed-4f40-a12d-719789c3484d"),
                titleEn: "3f354b88c4ff4eb3a89ee1a01baf627a21928efc0ad8440292a722be",
                titleAr: "4e79ff69861c45e49b839a59e32eb9184410534766e644a0b3f729371e0cf1fe8f4336fde8da4d3b9dd",
                subTitleEn: "a24054156acb497a9280215476afd0dae25ff057fabd4a73acb06c0b4",
                subTitleAr: "0e662d286d0142adb80ce040a8e4aa7aefa9a6de0653459fb028a2244593e1a8606",
                descriptionEn: "9ec7b10dc5024d9dadff92c8de664dd",
                descriptionAr: "d6e4427f880e",
                buttonUrl: "426354f44d02405b8fba",
                order: 482012038,
                isActive: true,
                amenityId: Guid.Parse("d2da12fa-8dc8-4585-a9b2-1e074fde2f9b")
            ));

            await _unitOfWorkManager.Current.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}