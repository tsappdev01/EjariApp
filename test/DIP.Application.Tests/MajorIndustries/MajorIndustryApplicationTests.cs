using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.MajorIndustries
{
    public class MajorIndustriesAppServiceTests : DIPApplicationTestBase
    {
        private readonly IMajorIndustriesAppService _majorIndustriesAppService;
        private readonly IRepository<MajorIndustry, Guid> _majorIndustryRepository;

        public MajorIndustriesAppServiceTests()
        {
            _majorIndustriesAppService = GetRequiredService<IMajorIndustriesAppService>();
            _majorIndustryRepository = GetRequiredService<IRepository<MajorIndustry, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _majorIndustriesAppService.GetListAsync(new GetMajorIndustriesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.MajorIndustry.Id == Guid.Parse("91857fcf-ca93-49ad-99b2-6614beb9f27b")).ShouldBe(true);
            result.Items.Any(x => x.MajorIndustry.Id == Guid.Parse("c85e452f-64fa-4b8a-bc92-36e72829b3ca")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _majorIndustriesAppService.GetAsync(Guid.Parse("91857fcf-ca93-49ad-99b2-6614beb9f27b"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("91857fcf-ca93-49ad-99b2-6614beb9f27b"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new MajorIndustryCreateDto
            {
                TitleEn = "7953b27f91a94108ab",
                TitleAr = "d72e1f47a77f47bda2716f4de19054d5b3e3b130831e4adb894f70504dcf5a70c3735f5716d94",
                Image = "ec8088e2baf44bc",
                Order = 92739841,
                IsActive = true,
                ZoneId = Guid.Parse("5968a2a9-f7fc-469f-904c-3f176982876e")
            };

            // Act
            var serviceResult = await _majorIndustriesAppService.CreateAsync(input);

            // Assert
            var result = await _majorIndustryRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("7953b27f91a94108ab");
            result.TitleAr.ShouldBe("d72e1f47a77f47bda2716f4de19054d5b3e3b130831e4adb894f70504dcf5a70c3735f5716d94");
            result.Image.ShouldBe("ec8088e2baf44bc");
            result.Order.ShouldBe(92739841);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new MajorIndustryUpdateDto()
            {
                TitleEn = "9818ab3865df4873a1690a345d03d39de7268d92fa1849af93ce4e54cc1535c7118f9201d2bd430cb34e",
                TitleAr = "49e6214a25244424b8dc24c68140fc2a8fead59664674e",
                Image = "a57ac73710514affae917a9e3364f484c00f56ab3658",
                Order = 1752333689,
                IsActive = true,
                ZoneId = Guid.Parse("5968a2a9-f7fc-469f-904c-3f176982876e")
            };

            // Act
            var serviceResult = await _majorIndustriesAppService.UpdateAsync(Guid.Parse("91857fcf-ca93-49ad-99b2-6614beb9f27b"), input);

            // Assert
            var result = await _majorIndustryRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("9818ab3865df4873a1690a345d03d39de7268d92fa1849af93ce4e54cc1535c7118f9201d2bd430cb34e");
            result.TitleAr.ShouldBe("49e6214a25244424b8dc24c68140fc2a8fead59664674e");
            result.Image.ShouldBe("a57ac73710514affae917a9e3364f484c00f56ab3658");
            result.Order.ShouldBe(1752333689);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _majorIndustriesAppService.DeleteAsync(Guid.Parse("91857fcf-ca93-49ad-99b2-6614beb9f27b"));

            // Assert
            var result = await _majorIndustryRepository.FindAsync(c => c.Id == Guid.Parse("91857fcf-ca93-49ad-99b2-6614beb9f27b"));

            result.ShouldBeNull();
        }
    }
}