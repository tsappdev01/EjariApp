using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.DipFacts
{
    public class DipFactsAppServiceTests : DIPApplicationTestBase
    {
        private readonly IDipFactsAppService _dipFactsAppService;
        private readonly IRepository<DipFact, Guid> _dipFactRepository;

        public DipFactsAppServiceTests()
        {
            _dipFactsAppService = GetRequiredService<IDipFactsAppService>();
            _dipFactRepository = GetRequiredService<IRepository<DipFact, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _dipFactsAppService.GetListAsync(new GetDipFactsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("cb55ae4c-b1ad-4e4d-afcb-2a75bb4c8641")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("0f18d8b1-5b9c-433a-869d-2856a56feb63")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _dipFactsAppService.GetAsync(Guid.Parse("cb55ae4c-b1ad-4e4d-afcb-2a75bb4c8641"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("cb55ae4c-b1ad-4e4d-afcb-2a75bb4c8641"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new DipFactCreateDto
            {
                Image = "74bc6d329536462da98466695ea3c34f85ef661d497",
                TitleAr = "31ed9805a6994b6eab3c51e84688b253ed2a43f096",
                TitleEn = "4dd1566fdaa8406f9948e3ea350eb2579dfa72cf9d9b49f5961b8a087c99d64a6759388e4ea54a0daaecc0d666073f",
                DescriptionAr = "b93932c423484d7fb612df8b331c58c0d871911bc8214e9c93d7b",
                DescriptionEn = "341f0e4b07754901868184ef459a11986b227118ce4",
                Order = 1004607290,
                IsActive = true
            };

            // Act
            var serviceResult = await _dipFactsAppService.CreateAsync(input);

            // Assert
            var result = await _dipFactRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Image.ShouldBe("74bc6d329536462da98466695ea3c34f85ef661d497");
            result.TitleAr.ShouldBe("31ed9805a6994b6eab3c51e84688b253ed2a43f096");
            result.TitleEn.ShouldBe("4dd1566fdaa8406f9948e3ea350eb2579dfa72cf9d9b49f5961b8a087c99d64a6759388e4ea54a0daaecc0d666073f");
            result.DescriptionAr.ShouldBe("b93932c423484d7fb612df8b331c58c0d871911bc8214e9c93d7b");
            result.DescriptionEn.ShouldBe("341f0e4b07754901868184ef459a11986b227118ce4");
            result.Order.ShouldBe(1004607290);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new DipFactUpdateDto()
            {
                Image = "fe66e09e",
                TitleAr = "4b618c6fdb714ee89f89d3375a667fcbcf6d7234763441108387fdc982433d96f0a",
                TitleEn = "6918ae69d5e74080b583",
                DescriptionAr = "c67a68b6db26406d8e80e1689fd0cdd55cf",
                DescriptionEn = "5dc19c1e73984eea9ea3f7bb130cb77e82ac0e2263b14b63ad1dd38e3b74e4b9c4cc4f41a8924f",
                Order = 1214997816,
                IsActive = true
            };

            // Act
            var serviceResult = await _dipFactsAppService.UpdateAsync(Guid.Parse("cb55ae4c-b1ad-4e4d-afcb-2a75bb4c8641"), input);

            // Assert
            var result = await _dipFactRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Image.ShouldBe("fe66e09e");
            result.TitleAr.ShouldBe("4b618c6fdb714ee89f89d3375a667fcbcf6d7234763441108387fdc982433d96f0a");
            result.TitleEn.ShouldBe("6918ae69d5e74080b583");
            result.DescriptionAr.ShouldBe("c67a68b6db26406d8e80e1689fd0cdd55cf");
            result.DescriptionEn.ShouldBe("5dc19c1e73984eea9ea3f7bb130cb77e82ac0e2263b14b63ad1dd38e3b74e4b9c4cc4f41a8924f");
            result.Order.ShouldBe(1214997816);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _dipFactsAppService.DeleteAsync(Guid.Parse("cb55ae4c-b1ad-4e4d-afcb-2a75bb4c8641"));

            // Assert
            var result = await _dipFactRepository.FindAsync(c => c.Id == Guid.Parse("cb55ae4c-b1ad-4e4d-afcb-2a75bb4c8641"));

            result.ShouldBeNull();
        }
    }
}