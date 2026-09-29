using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using DIP.FeedBacks;
using DIP.EntityFrameworkCore;
using Xunit;

namespace DIP.FeedBacks
{
    public class FeedBackRepositoryTests : DIPEntityFrameworkCoreTestBase
    {
        private readonly IFeedBackRepository _feedBackRepository;

        public FeedBackRepositoryTests()
        {
            _feedBackRepository = GetRequiredService<IFeedBackRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _feedBackRepository.GetListAsync(
                    subject: "0510f83af739422ca624576b8bc3d5c9579fb0f222ba43f48bd56f3448353",
                    companyName: "524d8bf486924a02871386e28640eed1ccc78ecb521141adb72ccb5355c7da6b92711e6edc34481fa",
                    plotNo: "16e505795f2447b58e1170857f02645fb68064279f854813a739655d0c5248a81275b0ef42f246d2a896aa096adf",
                    plotCategory: "c583a6f00c3",
                    contactPersonName: "9f0288c2d3074ac2bb4fd73d75f",
                    emailId: "62687@0d7fc.com",
                    mobileNumber: "c46d8a6e35954cd29066311a4941ed34b21e7278e01b4fba9f8d0a3e298bba69eb6a0066dc8848aabdff516aeff",
                    department: "d4f2b18ddd3747f78abbdeef642b4d069f5",
                    categoryName: "afddbe62479e4227b5c57",
                    description: "ef34c0ba646745a286fdde4a2"
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("0df1f9a1-94f5-4dd8-b366-7907cc36c9a3"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _feedBackRepository.GetCountAsync(
                    subject: "644da614c7a7430694f9fd29066fb2dcbae220589037402c89f04",
                    companyName: "2e4c41cc02d64ca",
                    plotNo: "0d243f22",
                    plotCategory: "daeaceeee3a3498fa851b",
                    contactPersonName: "29d00b17a4664215b17118",
                    emailId: "a29a79dad8954fd6a265536@1de396b9f4da4c639b65d9e.com",
                    mobileNumber: "5e6d08818a0e4581961b8af7003cb2f8b926a075f7d1437dab3353b299f8d4e339008",
                    department: "e32223d8ca99440fb32babf8785f2d7104da4798f3cc4aad9a6c5586d",
                    categoryName: "60ddb0156ebf402ca334792b54d49e3bbd99209b57de458eadb7ba4633562f",
                    description: "d7629e197f0a48d49ff9da84ac26b5ba31385ab7f866486"
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}