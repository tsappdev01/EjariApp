using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.DipBranches
{
    public class DipBranchesAppServiceTests : DIPApplicationTestBase
    {
        private readonly IDipBranchesAppService _dipBranchesAppService;
        private readonly IRepository<DipBranch, Guid> _dipBranchRepository;

        public DipBranchesAppServiceTests()
        {
            _dipBranchesAppService = GetRequiredService<IDipBranchesAppService>();
            _dipBranchRepository = GetRequiredService<IRepository<DipBranch, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _dipBranchesAppService.GetListAsync(new GetDipBranchesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("d908ca9e-93f2-41ec-8f0d-867385be2578")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("4ff3f561-d451-4b5f-a647-9b525ecc4f78")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _dipBranchesAppService.GetAsync(Guid.Parse("d908ca9e-93f2-41ec-8f0d-867385be2578"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("d908ca9e-93f2-41ec-8f0d-867385be2578"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new DipBranchCreateDto
            {
                TitleEn = "27d5f6d1155a460cae15058d76f878a8dff835d7e29f452393cd2fc0ee2229d689fd4a37c0634",
                TitleAr = "cce7afc8b6124a3785c230779ca4c5581e",
                SubTitleEn = "4ca63928cf9b49c0",
                SubTitleAr = "4c1f2125380f4c12963273463",
                Phone = "a048584537e74c0b8cd1ee300fb72d006cf59e0fcb21441ebec2768a0a4a59b3c15e",
                AlternativePhone = "a303d1ee561e4be29a0b098a315f",
                Email = "@.com",
                AlternativeEmail = "3a8ab424b75c44d6b586df8ff783c878146164947b9b42@7385a9870ca0466db5f002b491a90ee2713d873cce3b49.com",
                Order = 102091776,
                IsActive = true
            };

            // Act
            var serviceResult = await _dipBranchesAppService.CreateAsync(input);

            // Assert
            var result = await _dipBranchRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("27d5f6d1155a460cae15058d76f878a8dff835d7e29f452393cd2fc0ee2229d689fd4a37c0634");
            result.TitleAr.ShouldBe("cce7afc8b6124a3785c230779ca4c5581e");
            result.SubTitleEn.ShouldBe("4ca63928cf9b49c0");
            result.SubTitleAr.ShouldBe("4c1f2125380f4c12963273463");
            result.Phone.ShouldBe("a048584537e74c0b8cd1ee300fb72d006cf59e0fcb21441ebec2768a0a4a59b3c15e");
            result.AlternativePhone.ShouldBe("a303d1ee561e4be29a0b098a315f");
            result.Email.ShouldBe("@.com");
            result.AlternativeEmail.ShouldBe("3a8ab424b75c44d6b586df8ff783c878146164947b9b42@7385a9870ca0466db5f002b491a90ee2713d873cce3b49.com");
            result.Order.ShouldBe(102091776);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new DipBranchUpdateDto()
            {
                TitleEn = "25be0738eceb43d6b484dd7f6bd",
                TitleAr = "81bcd9b63a2d4ce29cabda8bcf0aae7ac76fba6e1dd74ad89097be9fcd080debef2b1a3ed68a4bf49aa3",
                SubTitleEn = "f444fc2aef60471f8c8e409352efc23753619659957248059f2e41405684d84930a5e4aa1dd64081a2083a7476321b12e13",
                SubTitleAr = "34b6ac8bba7349f58c5cbb8fa817d7423badfc37fd7847898c0071b2d524e96028989f69f9774731",
                Phone = "586344eaa13d4f4ca1ac2790dc42cf62f907796554324f1a8f14e982b56aca5de038dbde029a406e8b394e73fb0a92248",
                AlternativePhone = "c96adabb8",
                Email = "05a6918279ea4fa88b1257166@8d1490cde5374f55b58171d15.com",
                AlternativeEmail = "0798a599bcde4@1d7552b63a954.com",
                Order = 1152938532,
                IsActive = true
            };

            // Act
            var serviceResult = await _dipBranchesAppService.UpdateAsync(Guid.Parse("d908ca9e-93f2-41ec-8f0d-867385be2578"), input);

            // Assert
            var result = await _dipBranchRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("25be0738eceb43d6b484dd7f6bd");
            result.TitleAr.ShouldBe("81bcd9b63a2d4ce29cabda8bcf0aae7ac76fba6e1dd74ad89097be9fcd080debef2b1a3ed68a4bf49aa3");
            result.SubTitleEn.ShouldBe("f444fc2aef60471f8c8e409352efc23753619659957248059f2e41405684d84930a5e4aa1dd64081a2083a7476321b12e13");
            result.SubTitleAr.ShouldBe("34b6ac8bba7349f58c5cbb8fa817d7423badfc37fd7847898c0071b2d524e96028989f69f9774731");
            result.Phone.ShouldBe("586344eaa13d4f4ca1ac2790dc42cf62f907796554324f1a8f14e982b56aca5de038dbde029a406e8b394e73fb0a92248");
            result.AlternativePhone.ShouldBe("c96adabb8");
            result.Email.ShouldBe("05a6918279ea4fa88b1257166@8d1490cde5374f55b58171d15.com");
            result.AlternativeEmail.ShouldBe("0798a599bcde4@1d7552b63a954.com");
            result.Order.ShouldBe(1152938532);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _dipBranchesAppService.DeleteAsync(Guid.Parse("d908ca9e-93f2-41ec-8f0d-867385be2578"));

            // Assert
            var result = await _dipBranchRepository.FindAsync(c => c.Id == Guid.Parse("d908ca9e-93f2-41ec-8f0d-867385be2578"));

            result.ShouldBeNull();
        }
    }
}