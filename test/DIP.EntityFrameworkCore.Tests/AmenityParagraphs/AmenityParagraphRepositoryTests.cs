using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using DIP.AmenityParagraphs;
using DIP.EntityFrameworkCore;
using Xunit;

namespace DIP.AmenityParagraphs
{
    public class AmenityParagraphRepositoryTests : DIPEntityFrameworkCoreTestBase
    {
        private readonly IAmenityParagraphRepository _amenityParagraphRepository;

        public AmenityParagraphRepositoryTests()
        {
            _amenityParagraphRepository = GetRequiredService<IAmenityParagraphRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _amenityParagraphRepository.GetListAsync(
                    titleEn: "b0916c1c5795471194b2bc2fd300164f093951ac2d92459c811f56fb04ccbffba63ac95b",
                    titleAr: "c4416c6fdb6e4be",
                    subTitleEn: "5324b02eea04437c9cf6c903a3cba933ef829aa5ad754ca9abc7df98f1f54da19d85",
                    subTitleAr: "ed3ef49d96a0493e9072f7dc46521800fbbcd8a5c4c54405b1714a9e119ee7087888bf15f7ed41f5af0e94bcf2685f46",
                    descriptionEn: "c3caa01349f44d598438e92a44dbac6aab68f1856f7a4002aa7df76d8845585c38a0546e883",
                    descriptionAr: "d32b353822",
                    buttonUrl: "ded2c34cbea447369a6c88f6a939382c304cfd21b4",
                    isActive: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("e7f94fcc-8906-4dad-a3ce-045e04bc2bf4"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _amenityParagraphRepository.GetCountAsync(
                    titleEn: "3f354b88c4ff4eb3a89ee1a01baf627a21928efc0ad8440292a722be",
                    titleAr: "4e79ff69861c45e49b839a59e32eb9184410534766e644a0b3f729371e0cf1fe8f4336fde8da4d3b9dd",
                    subTitleEn: "a24054156acb497a9280215476afd0dae25ff057fabd4a73acb06c0b4",
                    subTitleAr: "0e662d286d0142adb80ce040a8e4aa7aefa9a6de0653459fb028a2244593e1a8606",
                    descriptionEn: "9ec7b10dc5024d9dadff92c8de664dd",
                    descriptionAr: "d6e4427f880e",
                    buttonUrl: "426354f44d02405b8fba",
                    isActive: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}