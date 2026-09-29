using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using DIP.EFormServices;
using DIP.EntityFrameworkCore;
using Xunit;

namespace DIP.EFormServices
{
    public class EFormServiceRepositoryTests : DIPEntityFrameworkCoreTestBase
    {
        private readonly IEFormServiceRepository _eFormServiceRepository;

        public EFormServiceRepositoryTests()
        {
            _eFormServiceRepository = GetRequiredService<IEFormServiceRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _eFormServiceRepository.GetListAsync(
                    titleEn: "50a255e8e1a245e394ca4d3cfd8d18d5ac5bd368",
                    titleAr: "56359f476381498298ce0732371fdcd168deceb70dd042",
                    isActive: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("f3a4fb45-a8e8-4d7b-9e5f-53cca28817ce"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _eFormServiceRepository.GetCountAsync(
                    titleEn: "71334fb8e71945af83cfc8f234c98a62e4670d0",
                    titleAr: "06ae111595fd41afbcbd9",
                    isActive: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}