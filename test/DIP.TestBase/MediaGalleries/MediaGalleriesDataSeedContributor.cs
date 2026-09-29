using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using DIP.MediaGalleries;

namespace DIP.MediaGalleries
{
    public class MediaGalleriesDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IMediaGalleryRepository _mediaGalleryRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public MediaGalleriesDataSeedContributor(IMediaGalleryRepository mediaGalleryRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _mediaGalleryRepository = mediaGalleryRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _mediaGalleryRepository.InsertAsync(new MediaGallery
            (
                id: Guid.Parse("22145e05-3258-4612-91df-358782e69b85"),
                titleEn: "f41fa4c3224f4b08b77b11e71b2bbaf947fa2608bd854c2d909ec80cf9d532e70ad060dfcf10427786997802dcd",
                slug: "d5e5729f39c24c3aa0b07a3c036037331fcf3ac407684d51bb2788d883356dbbe73b7400329a487",
                titleAr: "cbedc485ef2145f6b23068449432520d8b3c43ba36934c08b80b15f7e2034fd4532c390d3efd434",
                metaTitleEn: "603772e16af34229829aa82e3c8c4d2ec5bdf7e0cd1f467098ca5d93b1",
                metaTitleAr: "316eb17d92b04e8b8533317407e6b07ad92b04d160e74be9996745693c0a4ba7e2ebc291ddc6448",
                metaDescriptionEn: "3cde5c54e9d24c53ad69ce0597ecf772246e5b84548f4b9e912f3d46d4bcd8a17f03134",
                metaDescriptionAr: "f0157274afbd4ed983463ba2a0858",
                summaryEn: "033f569b50a7",
                summaryAr: "621f5048128040c998892249e8223c058cb765dd2",
                headerImage: "eb3e215ff2b7467cae47b33718163f878f6a5",
                image: "2c8bcc6269ea47f089e7b238a66bc16969a10b817bd34775b7812561d65bd23b6d0c15d04cae4c5fb59c641",
                order: 1196473715,
                isActive: true
            ));

            await _mediaGalleryRepository.InsertAsync(new MediaGallery
            (
                id: Guid.Parse("9aa50cb3-8d26-47de-9773-17b12f716eea"),
                titleEn: "3c9d20b81d7344169c98fc045ffabbb3f29b6ca673654f12bd5bd9b6a84f8a38e824951b61c14e409e13",
                slug: "9ba1da8b36764c47b2e04aa80e140e66c1e4",
                titleAr: "00ad70df687a4fa78eb2e1b28a222d7a0e0657040fab",
                metaTitleEn: "d2f677960cc84d5e9d9bc74c6064830241399b",
                metaTitleAr: "44663eb7f7304",
                metaDescriptionEn: "292c8eb560d74257bf8e3bc48ef9f524448fb4e9c",
                metaDescriptionAr: "8f6d65d6216648fa8d454c4cfdd46a0e7591619556974de0aece622e9e1e6c7509d8d023354f41f690de82396b301574",
                summaryEn: "9fe02b600d184f4b8e3bea7f0c0565ed2e1e3340e23e469ca1ed8b475575c2a0270156e011974a",
                summaryAr: "006e59c79e564169bf431a588416e458210f603bfa4546c7b5702eb888c47b33797902e3",
                headerImage: "7ba2a9e6fb744961a989dc0b2620f8cd",
                image: "2004d9ae09c54e469b819e8b366c05e25e6c23f64bf34cdd816a9",
                order: 587098976,
                isActive: true
            ));

            await _unitOfWorkManager.Current.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}