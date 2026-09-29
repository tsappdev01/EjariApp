using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using DIP.SupportedBanks;
using DIP.EntityFrameworkCore;
using Xunit;

namespace DIP.SupportedBanks
{
    public class SupportedBankRepositoryTests : DIPEntityFrameworkCoreTestBase
    {
        private readonly ISupportedBankRepository _supportedBankRepository;

        public SupportedBankRepositoryTests()
        {
            _supportedBankRepository = GetRequiredService<ISupportedBankRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _supportedBankRepository.GetListAsync(
                    titleAr: "2f1d8b1132074833b536",
                    titleEn: "4e4752ef94f64b5",
                    isActive: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("78b0094d-0ab7-4acc-ae8e-7fa0860ffc29"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _supportedBankRepository.GetCountAsync(
                    titleAr: "3baf40c230c94947b826eefb47b35b02a01b298cb846401091da6bba04cfe886fa989d0fe1b14750b4",
                    titleEn: "cb83c5feadcc45e",
                    isActive: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}