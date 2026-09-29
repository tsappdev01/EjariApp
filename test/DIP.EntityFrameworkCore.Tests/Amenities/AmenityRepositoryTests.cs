using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using DIP.Amenities;
using DIP.EntityFrameworkCore;
using Xunit;

namespace DIP.Amenities
{
    public class AmenityRepositoryTests : DIPEntityFrameworkCoreTestBase
    {
        private readonly IAmenityRepository _amenityRepository;

        public AmenityRepositoryTests()
        {
            _amenityRepository = GetRequiredService<IAmenityRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _amenityRepository.GetListAsync(
                    titleEn: "0c92a4d8c58c495a9867974ff3eef59437796fdb046846a288f054da56d2ef174789c33",
                    titleAr: "a666917b90104e11a01d6f37e825cddd214f425d88ed41338fecae3cb5e5344613b6f7d",
                    metaTitleEn: "6ba1953a1a6c4259b8",
                    metaDescriptionEn: "a79ceb1bf98a438a8432d48be730a1895b9594aaf9164692a8694b7",
                    metaTitleAr: "cc91135080f04b7983e05e0f8cd0e6055d3f",
                    metaDescriptionAr: "97b5cb263969",
                    slug: "282fa2b91a7044dba142a804dfb4b1936cef4008fe444b6c944af1",
                    headerImage: "d0a5c1ee52ca42fbbf05718ab617cb7965c6f38435b845f3acaf9a7dddb06b821f5",
                    isActive: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("81959370-0ccd-4dda-9464-17eaa13ac3a4"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _amenityRepository.GetCountAsync(
                    titleEn: "8fc16795709f484baea510",
                    titleAr: "096ff57cd31349129826cd9efc3ac033af976610e44449389",
                    metaTitleEn: "46606a1d148749d39ed18fddd1ef552ac1da6c17942d434fb48ceb8fce1336cb42dd8",
                    metaDescriptionEn: "c67db034323247b6b5872bf80ea00f73aeeea62afd6d4",
                    metaTitleAr: "4da0328efa914faa91477f122e29e481cc9528bc73624718863251d68e6e095292075227721f40768409896ae5",
                    metaDescriptionAr: "238f5a62720742d9a34b03834c0df9bdbc3d96b49292400",
                    slug: "420bec6526",
                    headerImage: "99d201eedcd747c698589dab64a549638de11da9a65445d8bf47c634e08787824529",
                    isActive: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}