using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using DIP.Commercials;
using DIP.EntityFrameworkCore;
using Xunit;

namespace DIP.Commercials
{
    public class CommercialRepositoryTests : DIPEntityFrameworkCoreTestBase
    {
        private readonly ICommercialRepository _commercialRepository;

        public CommercialRepositoryTests()
        {
            _commercialRepository = GetRequiredService<ICommercialRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _commercialRepository.GetListAsync(
                    titleEn: "a44175f61b3a42bfb2499f53e82c6d81c527da10a2e3440994f616448a",
                    titleAr: "01489a51de5041cf8957b541617e88f2bca60b28c05b4618ba665862d0ddd0f0bbf7f9532f9a42fd98866aded86020",
                    plotNo: "85c24879",
                    activityEn: "cb21644a921d45",
                    activityAr: "978b3a57e5c4485091b6c48996ee5cec69aa0",
                    phone: "242023501c0a461595f054738716e7f7d86a674e263045b0a8206",
                    fax: "c18ea878afc74141a1ccbc10e57",
                    isActive: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("643289cf-49ba-4b11-8aa8-e229cafdfca8"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _commercialRepository.GetCountAsync(
                    titleEn: "23fb193b4a1f451c92499db3e04789a871246b5208ab",
                    titleAr: "67481d410ee",
                    plotNo: "58342a947ddd4b9f9db",
                    activityEn: "bc52e85474ac425b86d1a16ce3ea2d7740348fa2255247c89c",
                    activityAr: "bf9ede67212d4668a4288078177784a1e048a4fb3efa4f418ee7a06474be400b6adf27358982450890c314b51a4a50b50f",
                    phone: "5b25e6a5e92f4f688f8351a19c17c40e267ea8c690ee45a490a74819",
                    fax: "65ebd554c0794dc284fa803e38e2c3dd0f48314e5c3045cbb021e0aa0191e6ab3d7f298e6bd144cf9cdbef1",
                    isActive: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}