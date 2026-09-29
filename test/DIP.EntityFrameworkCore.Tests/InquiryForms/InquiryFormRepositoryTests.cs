using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using DIP.InquiryForms;
using DIP.EntityFrameworkCore;
using Xunit;

namespace DIP.InquiryForms
{
    public class InquiryFormRepositoryTests : DIPEntityFrameworkCoreTestBase
    {
        private readonly IInquiryFormRepository _inquiryFormRepository;

        public InquiryFormRepositoryTests()
        {
            _inquiryFormRepository = GetRequiredService<IInquiryFormRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _inquiryFormRepository.GetListAsync(
                    companyName: "1bb0d1bdf7094c659973e96fb6a5313e121dc3a8d45144fdb7f127687e325da885",
                    name: "051cf206ab9a4216a86",
                    email: "e3e@a12.com",
                    mobile: "234d663c105b41809ef84f9c3b70c6b98a4c8a65bf3145c6953e3898c4377fd6d6",
                    fax: "79339b6df06d4c52bbb985ad9b68a4ccf7e08059969a48e38790c36b8c827e53",
                    phone: "142b9ce23e0143c782b40921cb33d2d4091c88acce504633b1d82077c5da0829fb20c97b3",
                    inquiryType: "d3875274afa141fd9a251a14c0fc1ecb842d9aa31a2e43e588177d1dca808ee4fa24e89b6b1c413ab9383fdf",
                    tradeLicensePlateOfIssue: "5e68fb7c173f",
                    buyRent: "3d363c18fbae4",
                    comments: "5cb6fc5635d541a680f701a17e50d7a4c9c81f81b66840df99f5301eb51ed475"
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("0282fa84-8efb-48fd-9818-684f3e2056ec"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _inquiryFormRepository.GetCountAsync(
                    companyName: "3519dd309d55485187ac94ec023a00ed",
                    name: "5580c480c7464d0c90391b0b59043153d9cbc42ef12b40e2893deb767355718afb2a3a5c86a2433492edc6f9da04529cdc8",
                    email: "c10e481f30494e119@50ef0e0f3b924b7fb.com",
                    mobile: "3a7e8d5619924c95b93ae456f477e43fb6b5ebde32cb4d29a",
                    fax: "9b12ad94924443e3a9725a945093f2b220fe77c1506944738987b4a24c0e68851d9e1e15b8da4365b4c83dddb5751f45",
                    phone: "d181f071b60547e1b008070324071d645d86bf7484104672b51ee3c64e08801299c9b2d31d4f454cbd5e00eabcaeb1490f",
                    inquiryType: "46c42f33e1ac4d1fb0f0354643c01d11b2",
                    tradeLicensePlateOfIssue: "93be74a4b95c4a8dbe69e40aa40c5bdffe9c0a85e798482e9b08f1dec0bc3df",
                    buyRent: "961345c597d8435ca44ea539a35a179c4250485124dd443883b43a23b4fd91923fd33b1f4c2a48299d34eb",
                    comments: "6bb089c986fc432894ada9a456a1d1891cd71c11dd29429ba2ab"
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}