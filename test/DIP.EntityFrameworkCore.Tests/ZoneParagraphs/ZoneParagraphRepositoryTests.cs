using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using DIP.ZoneParagraphs;
using DIP.EntityFrameworkCore;
using Xunit;

namespace DIP.ZoneParagraphs
{
    public class ZoneParagraphRepositoryTests : DIPEntityFrameworkCoreTestBase
    {
        private readonly IZoneParagraphRepository _zoneParagraphRepository;

        public ZoneParagraphRepositoryTests()
        {
            _zoneParagraphRepository = GetRequiredService<IZoneParagraphRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _zoneParagraphRepository.GetListAsync(
                    titleEn: "29824677252849f4834",
                    titleAr: "2b3046adf5f64f13877166761200cfbbc88a252f5b0",
                    subTilteEn: "1ba080c3e8c348cabd9e043ff92eb",
                    subTitleAr: "79a4329ef7744eb080f314a92d11c656650709b3",
                    descriptionEn: "bccd2c4d4e3746ab824d59e48921a30a",
                    descriptionAr: "2e59cbe1521e4d2387f880c6ae2f5458ad",
                    isActive: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("a289ab56-f9c9-476d-aed3-4fb91d7d14be"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _zoneParagraphRepository.GetCountAsync(
                    titleEn: "81746003b6894640861eb4e9ec5f6",
                    titleAr: "c5401e500dc542599ef4c620b6c232e4498a0ac371cb49579aa87672ec42868620dcc7b3310f41c5a033",
                    subTilteEn: "bbfbc4773490433889b77653d3feab6d3e086a8702b94edba0fe77bcd717fe770661f35a49544a70b27a53f773bc3",
                    subTitleAr: "775051f946c04085a72802f6f535606f164c015d20924afba4f87b64667aea30",
                    descriptionEn: "7a04fd10cf15",
                    descriptionAr: "bfa3f3d85a874ece962921f7288ec5243f96072d7feb4076a8d536d1e536f23761f1455fc58d4c699820",
                    isActive: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}