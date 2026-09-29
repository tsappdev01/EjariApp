using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using DIP.ContactForms;
using DIP.EntityFrameworkCore;
using Xunit;

namespace DIP.ContactForms
{
    public class ContactFormRepositoryTests : DIPEntityFrameworkCoreTestBase
    {
        private readonly IContactFormRepository _contactFormRepository;

        public ContactFormRepositoryTests()
        {
            _contactFormRepository = GetRequiredService<IContactFormRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _contactFormRepository.GetListAsync(
                    fullName: "46cd91f708564ece9f91cb36fbf64ead1593c05b2d8f4ff394f1792ef4040fa",
                    email: "6bd9cccc3d@f14dfef275.com",
                    subject: "74034960cc1240b0ab231fdd1d0a93a55e6c259251ee49d484d",
                    message: "e42d16ded61243659f38c66f6173adea4e13b539d11246b3808f80a323"
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("d7dd5e88-358c-4746-a8e7-0df1a50e5350"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _contactFormRepository.GetCountAsync(
                    fullName: "ae44791550b347a6b0faa5783702bd1b22ae88d740474ffe861b1f91e2c36203b6f73d2eff704af4bd7da2b",
                    email: "0bf47f4afdf64@9c6ae9782ed34.com",
                    subject: "1992610b7295401f80a9cca779d73cd1de955127eaa345cf8ba2ecc",
                    message: "21b9594cc0ff434a970f4e0c9ee14a74dca47d0d0c1a4346a7068a"
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}