using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using DIP.MajorIndustries;
using DIP.EntityFrameworkCore;
using Xunit;

namespace DIP.MajorIndustries
{
    public class MajorIndustryRepositoryTests : DIPEntityFrameworkCoreTestBase
    {
        private readonly IMajorIndustryRepository _majorIndustryRepository;

        public MajorIndustryRepositoryTests()
        {
            _majorIndustryRepository = GetRequiredService<IMajorIndustryRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _majorIndustryRepository.GetListAsync(
                    titleEn: "824d12d3626641d38c10d74a39e520ef63d7f55286ae48c59bab84f4d9eeb84ec2f2ffd7f9be4e0f85082ce709d4f4ca",
                    titleAr: "d0994343dc8b4cf8b086d6d79bb5e5e00dc00ff642544",
                    image: "0c1c663358a244bbb1f970efcf8b0eee8ade8e4bb23647baaed39843d",
                    isActive: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("91857fcf-ca93-49ad-99b2-6614beb9f27b"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _majorIndustryRepository.GetCountAsync(
                    titleEn: "22e2197f534b476eb0b211a3c45465c51fefc88569bb4ab2bdb390b8da42ea4d5162527252",
                    titleAr: "eec52b1fa370462fbe82c28417a7e2be5999de3031404b0a81dc4f4e3fd25523dab754f3",
                    image: "431fc7f21fdc4876b8d46425ef9de5512b693f054cfd40b291",
                    isActive: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}