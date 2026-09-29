using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using DIP.DipFacts;
using DIP.EntityFrameworkCore;
using Xunit;

namespace DIP.DipFacts
{
    public class DipFactRepositoryTests : DIPEntityFrameworkCoreTestBase
    {
        private readonly IDipFactRepository _dipFactRepository;

        public DipFactRepositoryTests()
        {
            _dipFactRepository = GetRequiredService<IDipFactRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _dipFactRepository.GetListAsync(
                    image: "f395f8927d944ed1a936386579cdcaaa00bf70bb4",
                    titleAr: "132d8342757c49e3ae9b8f179c6880d8ec42",
                    titleEn: "21082e50285449ca9e942ac6094b56eba49ec67e0c0d4ea8a57913780aa1f6aa135a816a5f7b4ad193e3050325652",
                    descriptionAr: "d50ba2bd93b64fce837182831e7408dbb1830b6f912a49309b4b53811c1dfc89dcb",
                    descriptionEn: "62727e85d4504a29afd8304e7d1e24e271822a7d743c4927802e6b4834",
                    isActive: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("cb55ae4c-b1ad-4e4d-afcb-2a75bb4c8641"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _dipFactRepository.GetCountAsync(
                    image: "bd296b65de7d4dc58d89234dac461ac84cb9e95252e147f4b75",
                    titleAr: "1061392894424c0f8b407b25856106dba6d4ee4fd5b5438ca7590cf4d914458e",
                    titleEn: "baf0054b2db7427e9f063abc3e6c8354fdc6cc2d21f34a8f836c95af06e4ff1a5d65c59855be4caf9b4eb0f15f46237d",
                    descriptionAr: "cea483d028f44519a1a73462925fb3b2ebef386b1aff45b09edab0ebec",
                    descriptionEn: "1e39e7a72e2b44f4aed4ddda5fd1ffdaef590ad84b5a4e53b6380a8f91b7b13d7e82113848134c7aa144e5909",
                    isActive: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}