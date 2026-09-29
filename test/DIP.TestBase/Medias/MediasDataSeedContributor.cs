using DIP.MediaGalleries;
using DIP.AmenityParagraphs;
using DIP.ZoneParagraphs;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using DIP.Medias;

namespace DIP.Medias
{
    public class MediasDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IMediaRepository _mediaRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ZoneParagraphsDataSeedContributor _zoneParagraphsDataSeedContributor;

        private readonly AmenityParagraphsDataSeedContributor _amenityParagraphsDataSeedContributor;

        private readonly MediaGalleriesDataSeedContributor _mediaGalleriesDataSeedContributor;

        public MediasDataSeedContributor(IMediaRepository mediaRepository, IUnitOfWorkManager unitOfWorkManager, ZoneParagraphsDataSeedContributor zoneParagraphsDataSeedContributor, AmenityParagraphsDataSeedContributor amenityParagraphsDataSeedContributor, MediaGalleriesDataSeedContributor mediaGalleriesDataSeedContributor)
        {
            _mediaRepository = mediaRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _zoneParagraphsDataSeedContributor = zoneParagraphsDataSeedContributor; _amenityParagraphsDataSeedContributor = amenityParagraphsDataSeedContributor; _mediaGalleriesDataSeedContributor = mediaGalleriesDataSeedContributor;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _zoneParagraphsDataSeedContributor.SeedAsync(context);
            await _amenityParagraphsDataSeedContributor.SeedAsync(context);
            await _mediaGalleriesDataSeedContributor.SeedAsync(context);

            await _mediaRepository.InsertAsync(new Media
            (
                id: Guid.Parse("4a7b9266-2cd3-4bbb-bf62-8043e8eb54ed"),
                titleEn: "1827743a84064c6cb2e8eeec4cdde0949c4aa62",
                titleAr: "8f2c95ebc25e4733928b8fb9e6644a2b9bde5863a6444f80a296cc08299d15f8720ba5510e424d76a55368deadf5011",
                file: "508fd20023a24a3bbbb0852d599a8ee41bb38d5c56314c329f38a79565aa5247f04a550677a342ff846345fca01",
                order: 2135242461,
                isActive: true,
                zoneParagraphId: null,
                amenityParagraphId: null,
                mediaGalleryId: null
            ));

            await _mediaRepository.InsertAsync(new Media
            (
                id: Guid.Parse("77639ba9-cba5-46a1-9605-0eda1b8a4500"),
                titleEn: "08147c1b3f8444d9a404c686f3885e248f7205973dae42ada4e917c52868338df55cf3",
                titleAr: "0e444fa108884c508fd2c064ba5f79711828acd4832b4b38813d",
                file: "b37cd37c3a894b6fa2c27970b95bd7c74184fe46148e4787a3734ae",
                order: 323768589,
                isActive: true,
                zoneParagraphId: null,
                amenityParagraphId: null,
                mediaGalleryId: null
            ));

            await _unitOfWorkManager.Current.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}