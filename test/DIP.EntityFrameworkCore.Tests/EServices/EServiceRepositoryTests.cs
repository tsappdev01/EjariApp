using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using DIP.EServices;
using DIP.EntityFrameworkCore;
using Xunit;

namespace DIP.EServices
{
    public class EServiceRepositoryTests : DIPEntityFrameworkCoreTestBase
    {
        private readonly IEServiceRepository _eServiceRepository;

        public EServiceRepositoryTests()
        {
            _eServiceRepository = GetRequiredService<IEServiceRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _eServiceRepository.GetListAsync(
                    titleEn: "5bb6d7b2028142329f5088a83f252f49473b85b58db741598e5de296a5418db998",
                    titleAr: "09bb516ad49248428597888c83e7486cbda37cccdea74238bf1f5b346f707ab92942dc6bf8274db9b9e1c62179f7ef",
                    slug: "a0f0e48cc2d844bba3c8b66eabfa19300f8911f907504eedbd78ee90908dc67fcd51",
                    descriptionEn: "b549f29f46704b08a43a742403a2b3da933a298d6d5947d09b0f1a967c0c9ab51e2e1b789cf945ec854796c4cca6d67416",
                    descriptionAr: "bbaa7b891b1d4b7d9efcda7ac2d1079dd1ed655cd6e341b1b445555d6e4a0c4af581742a0d9f4b4cad6f674fc0",
                    image: "c11cc51c3dc5487ba664cfa0757eff8ecd805773296a4c5f88e2f8345f4f07aff2a5a346",
                    headerImage: "a9bc1b54fc544eb28192fd12a5ca7d1fdd8c606335",
                    metaTitleEn: "d981c595c1e941ca8e433fb392d6fb878206b64bf3a044d78de5eb89689e4653a5c",
                    metaTitleAr: "955182873e2b43e5a560d45e44adee92baea8b33fcfb4b89a9794cbad9deb0cbca6dd8b30",
                    metaDescriptionEn: "1186ebd21ad",
                    metaDescriptionAr: "0160f1845b734c5a80a7ede1204",
                    isActive: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("7b1f4893-4fa5-419b-9369-9962f6a937aa"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _eServiceRepository.GetCountAsync(
                    titleEn: "279dd308ee0348",
                    titleAr: "f38abaf16c4544fd941addb9ef034b0f92a81a55df824bce8366dcef022d757a61b605b8b8d34ae99a86",
                    slug: "ec5e93d2152540f2bcab1f29dbc4a544",
                    descriptionEn: "fb04c26b87ae44b29347c60cfffe784b10f7dc046919433c88df54dc9b1",
                    descriptionAr: "b415539f1c804ec3884cbe520869975c13e0a29e500445569a0dce7f7da00",
                    image: "f31876feceb14361a73dc506ed14ad08d073e18547",
                    headerImage: "91051ec0d5874383b3877e43fd8074e58ff5023fc010450a937a8d6f69dc231a2e3fdf4c14e1456a95912152e701",
                    metaTitleEn: "03c0f8f3260d4",
                    metaTitleAr: "01b9ee366a1d4d6d9e88837b7809a906fb2eec925beb4adf965ea2a73ef117ddfed7b176",
                    metaDescriptionEn: "f6e9644f690d4fee9",
                    metaDescriptionAr: "4c74a86c7e8747e6b694de535a7732c95207bc16f2f5400891a0715190c23c3d4191ddba16a843",
                    isActive: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}