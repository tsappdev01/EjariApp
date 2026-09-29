using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using DIP.Medias;
using DIP.EntityFrameworkCore;
using Xunit;

namespace DIP.Medias
{
    public class MediaRepositoryTests : DIPEntityFrameworkCoreTestBase
    {
        private readonly IMediaRepository _mediaRepository;

        public MediaRepositoryTests()
        {
            _mediaRepository = GetRequiredService<IMediaRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _mediaRepository.GetListAsync(
                    titleEn: "1827743a84064c6cb2e8eeec4cdde0949c4aa62",
                    titleAr: "8f2c95ebc25e4733928b8fb9e6644a2b9bde5863a6444f80a296cc08299d15f8720ba5510e424d76a55368deadf5011",
                    file: "508fd20023a24a3bbbb0852d599a8ee41bb38d5c56314c329f38a79565aa5247f04a550677a342ff846345fca01",
                    isActive: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("4a7b9266-2cd3-4bbb-bf62-8043e8eb54ed"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _mediaRepository.GetCountAsync(
                    titleEn: "08147c1b3f8444d9a404c686f3885e248f7205973dae42ada4e917c52868338df55cf3",
                    titleAr: "0e444fa108884c508fd2c064ba5f79711828acd4832b4b38813d",
                    file: "b37cd37c3a894b6fa2c27970b95bd7c74184fe46148e4787a3734ae",
                    isActive: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}