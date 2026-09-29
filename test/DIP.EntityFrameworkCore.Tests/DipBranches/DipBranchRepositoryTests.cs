using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using DIP.DipBranches;
using DIP.EntityFrameworkCore;
using Xunit;

namespace DIP.DipBranches
{
    public class DipBranchRepositoryTests : DIPEntityFrameworkCoreTestBase
    {
        private readonly IDipBranchRepository _dipBranchRepository;

        public DipBranchRepositoryTests()
        {
            _dipBranchRepository = GetRequiredService<IDipBranchRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _dipBranchRepository.GetListAsync(
                    titleEn: "2a2df261181c4b2a8b91a8d86db1b77be2b236099f1d4368a0389956d982880682348579143a4a4",
                    titleAr: "e002f6ae0936473da38889fd5b46ea5f2899b5b8a8f14",
                    subTitleEn: "7b75093f063a4703ab5432ba5379fc1733d5e352614a4661ad5087c79807d005f44062f45f08462495",
                    subTitleAr: "af608566b91143dba6c4ac1f61829c6024c1ce206dac4be995790eac29d6c85111107c5bf41445a19e9826a95a44b54f1",
                    phone: "8e0175e9a61d44d09688fde686c",
                    alternativePhone: "9c01204ab8d045289698da60a4c444b98e0b12c7",
                    email: "e375fd463f3244bbbd66ab887b@53df6d1eb8364342b7cbfa47aa.com",
                    alternativeEmail: "@.com",
                    isActive: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("d908ca9e-93f2-41ec-8f0d-867385be2578"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _dipBranchRepository.GetCountAsync(
                    titleEn: "3ef6c2e177024d20a824d1624",
                    titleAr: "9e8e482a971a45cdacf666c6805904336f22a69da6a94b3e",
                    subTitleEn: "1597ee2702ea47dbba2fd7cba62ef6c",
                    subTitleAr: "019a060c55f94b86bc272b37126e11916d50c8337cf343cc9f5aeb9742f41b0f1f97",
                    phone: "dbdb2c749aad43af9d5391c9f9733e143566a448735e420481604b3056d78757b",
                    alternativePhone: "317b7e6a086c4ab4956d0e033bfa05c6b02aee3161b848cabd2eb0d71a4d78",
                    email: "3ad962967e2d4a949c25633fb9e82@681c4cdb9a4a4a0bbaffeefde36ba.com",
                    alternativeEmail: "f9cb208df87d4785b9c@aa75646a592b461195c.com",
                    isActive: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}