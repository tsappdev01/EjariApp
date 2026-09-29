using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.SupportedBanks
{
    public class SupportedBanksAppServiceTests : DIPApplicationTestBase
    {
        private readonly ISupportedBanksAppService _supportedBanksAppService;
        private readonly IRepository<SupportedBank, Guid> _supportedBankRepository;

        public SupportedBanksAppServiceTests()
        {
            _supportedBanksAppService = GetRequiredService<ISupportedBanksAppService>();
            _supportedBankRepository = GetRequiredService<IRepository<SupportedBank, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _supportedBanksAppService.GetListAsync(new GetSupportedBanksInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("78b0094d-0ab7-4acc-ae8e-7fa0860ffc29")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("e0b9d3dd-d1f6-40ff-8ec7-83ad1910684e")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _supportedBanksAppService.GetAsync(Guid.Parse("78b0094d-0ab7-4acc-ae8e-7fa0860ffc29"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("78b0094d-0ab7-4acc-ae8e-7fa0860ffc29"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new SupportedBankCreateDto
            {
                TitleAr = "f1173427cb80423499e7987aa4046b4",
                TitleEn = "3dcde98a789d471bb42ade5c3b3aff79d5b114e7ca354ef2a874096586a2e6c77129",
                IsActive = true,
                Order = 1192718581
            };

            // Act
            var serviceResult = await _supportedBanksAppService.CreateAsync(input);

            // Assert
            var result = await _supportedBankRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleAr.ShouldBe("f1173427cb80423499e7987aa4046b4");
            result.TitleEn.ShouldBe("3dcde98a789d471bb42ade5c3b3aff79d5b114e7ca354ef2a874096586a2e6c77129");
            result.IsActive.ShouldBe(true);
            result.Order.ShouldBe(1192718581);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new SupportedBankUpdateDto()
            {
                TitleAr = "ccbe2b6aa84e418ab9cced33c411d",
                TitleEn = "0dd3fbf89ef64a8794b043440bf06",
                IsActive = true,
                Order = 67994999
            };

            // Act
            var serviceResult = await _supportedBanksAppService.UpdateAsync(Guid.Parse("78b0094d-0ab7-4acc-ae8e-7fa0860ffc29"), input);

            // Assert
            var result = await _supportedBankRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleAr.ShouldBe("ccbe2b6aa84e418ab9cced33c411d");
            result.TitleEn.ShouldBe("0dd3fbf89ef64a8794b043440bf06");
            result.IsActive.ShouldBe(true);
            result.Order.ShouldBe(67994999);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _supportedBanksAppService.DeleteAsync(Guid.Parse("78b0094d-0ab7-4acc-ae8e-7fa0860ffc29"));

            // Assert
            var result = await _supportedBankRepository.FindAsync(c => c.Id == Guid.Parse("78b0094d-0ab7-4acc-ae8e-7fa0860ffc29"));

            result.ShouldBeNull();
        }
    }
}