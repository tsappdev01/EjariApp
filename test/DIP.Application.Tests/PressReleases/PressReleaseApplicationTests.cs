using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.PressReleases
{
    public class PressReleasesAppServiceTests : DIPApplicationTestBase
    {
        private readonly IPressReleasesAppService _pressReleasesAppService;
        private readonly IRepository<PressRelease, Guid> _pressReleaseRepository;

        public PressReleasesAppServiceTests()
        {
            _pressReleasesAppService = GetRequiredService<IPressReleasesAppService>();
            _pressReleaseRepository = GetRequiredService<IRepository<PressRelease, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _pressReleasesAppService.GetListAsync(new GetPressReleasesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("692c1e29-d6e7-4ec8-90d5-1b86615e0989")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("52baeeb7-5b74-45bd-ac93-dc8d53461ba5")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _pressReleasesAppService.GetAsync(Guid.Parse("692c1e29-d6e7-4ec8-90d5-1b86615e0989"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("692c1e29-d6e7-4ec8-90d5-1b86615e0989"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new PressReleaseCreateDto
            {
                TitleAr = "db8cc7a7fd9c4676a9096213f3",
                TitleEn = "b2020e4f6e4e4d7581edfe7057",
                MetaTitleAr = "b5be84dfc3c04",
                MetaTitleEn = "a6f424dcfd7c4df5a0063f6faaa36b127f7e79a3c3d046c2a",
                MetaDescriptionEn = "975a8d2d2ce7493",
                MetaDescriptionAr = "0c554c2b4cd9487295f067d9bfdc0a98b0c60e447c554b958c1d5affd6c2e881fbaac8335c",
                IsFeatured = true,
                Slug = "4026e13",
                Image = "c85b7a4ab390456ea2dee0d686",
                HeaderImage = "3f188336b4034172af6a7923a3ecd70771b56fe6f70142a3b06c91a4b7446fa5f149aa122c3a4888a1e",
                DescriptionAr = "dc78a222c08c4fb39c30d789946607024135a3cdf42240c59b301acaf2a8c6447",
                DescriptionEn = "84a94178ed9b4bef88132ed14293b7f36c",
                SummaryEn = "0bae8fdb543a4e08a8dc492bb118cbaf7",
                SummaryAr = "f0affdd1391f426fbece67b6070c",
                Order = 130877283,
                Date = new DateTime(2005, 3, 13),
                IsActive = true
            };

            // Act
            var serviceResult = await _pressReleasesAppService.CreateAsync(input);

            // Assert
            var result = await _pressReleaseRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleAr.ShouldBe("db8cc7a7fd9c4676a9096213f3");
            result.TitleEn.ShouldBe("b2020e4f6e4e4d7581edfe7057");
            result.MetaTitleAr.ShouldBe("b5be84dfc3c04");
            result.MetaTitleEn.ShouldBe("a6f424dcfd7c4df5a0063f6faaa36b127f7e79a3c3d046c2a");
            result.MetaDescriptionEn.ShouldBe("975a8d2d2ce7493");
            result.MetaDescriptionAr.ShouldBe("0c554c2b4cd9487295f067d9bfdc0a98b0c60e447c554b958c1d5affd6c2e881fbaac8335c");
            result.IsFeatured.ShouldBe(true);
            result.Slug.ShouldBe("4026e13");
            result.Image.ShouldBe("c85b7a4ab390456ea2dee0d686");
            result.HeaderImage.ShouldBe("3f188336b4034172af6a7923a3ecd70771b56fe6f70142a3b06c91a4b7446fa5f149aa122c3a4888a1e");
            result.DescriptionAr.ShouldBe("dc78a222c08c4fb39c30d789946607024135a3cdf42240c59b301acaf2a8c6447");
            result.DescriptionEn.ShouldBe("84a94178ed9b4bef88132ed14293b7f36c");
            result.SummaryEn.ShouldBe("0bae8fdb543a4e08a8dc492bb118cbaf7");
            result.SummaryAr.ShouldBe("f0affdd1391f426fbece67b6070c");
            result.Order.ShouldBe(130877283);
            result.Date.ShouldBe(new DateTime(2005, 3, 13));
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new PressReleaseUpdateDto()
            {
                TitleAr = "c5b64d0d1e8f44bdaf89138202e511c128f0c04db03e40e2b6464ce6c31f2344d490c9025",
                TitleEn = "f88cd2c111b648dc9256237037490803d8396d71209f4b1",
                MetaTitleAr = "9aa7aeb3945045cebbdb61e78b6fe",
                MetaTitleEn = "876b64f5fc054e24a58c30d38bcaea9bc30d445d13a94e48a749b62046dfe6ba670773635e9746458130302846b851",
                MetaDescriptionEn = "88ac4245ea7544dcbb634c678e54c130409c65dda220431386",
                MetaDescriptionAr = "b4d5c07bfdf3413",
                IsFeatured = true,
                Slug = "d290a48be39f4fe68b0893896e06a4bac3e854a00ad34735816e9e70a2003ce088f0c67be6f14b7",
                Image = "acc56fd2d450411f8dc",
                HeaderImage = "39de019d00dc4b02a73959faefdb266d6904bf6066914fc98",
                DescriptionAr = "1e36642d29d847c49f9147c5751446b1e89bdbc33ef34bfc841ffde0711a6dd758cff95639ac49",
                DescriptionEn = "746621bea3f9",
                SummaryEn = "136717a94d1b424db83e4f7e7c80acf954dcffb093134e388951e6ea1337a1f6636f4cb4a1ec43a69ecdd9fe06852cf0",
                SummaryAr = "121b997e85a5425ea59955dd31957c5503f5465c58654e22a77827de0eb8f0",
                Order = 327833954,
                Date = new DateTime(2014, 1, 18),
                IsActive = true
            };

            // Act
            var serviceResult = await _pressReleasesAppService.UpdateAsync(Guid.Parse("692c1e29-d6e7-4ec8-90d5-1b86615e0989"), input);

            // Assert
            var result = await _pressReleaseRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleAr.ShouldBe("c5b64d0d1e8f44bdaf89138202e511c128f0c04db03e40e2b6464ce6c31f2344d490c9025");
            result.TitleEn.ShouldBe("f88cd2c111b648dc9256237037490803d8396d71209f4b1");
            result.MetaTitleAr.ShouldBe("9aa7aeb3945045cebbdb61e78b6fe");
            result.MetaTitleEn.ShouldBe("876b64f5fc054e24a58c30d38bcaea9bc30d445d13a94e48a749b62046dfe6ba670773635e9746458130302846b851");
            result.MetaDescriptionEn.ShouldBe("88ac4245ea7544dcbb634c678e54c130409c65dda220431386");
            result.MetaDescriptionAr.ShouldBe("b4d5c07bfdf3413");
            result.IsFeatured.ShouldBe(true);
            result.Slug.ShouldBe("d290a48be39f4fe68b0893896e06a4bac3e854a00ad34735816e9e70a2003ce088f0c67be6f14b7");
            result.Image.ShouldBe("acc56fd2d450411f8dc");
            result.HeaderImage.ShouldBe("39de019d00dc4b02a73959faefdb266d6904bf6066914fc98");
            result.DescriptionAr.ShouldBe("1e36642d29d847c49f9147c5751446b1e89bdbc33ef34bfc841ffde0711a6dd758cff95639ac49");
            result.DescriptionEn.ShouldBe("746621bea3f9");
            result.SummaryEn.ShouldBe("136717a94d1b424db83e4f7e7c80acf954dcffb093134e388951e6ea1337a1f6636f4cb4a1ec43a69ecdd9fe06852cf0");
            result.SummaryAr.ShouldBe("121b997e85a5425ea59955dd31957c5503f5465c58654e22a77827de0eb8f0");
            result.Order.ShouldBe(327833954);
            result.Date.ShouldBe(new DateTime(2014, 1, 18));
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _pressReleasesAppService.DeleteAsync(Guid.Parse("692c1e29-d6e7-4ec8-90d5-1b86615e0989"));

            // Assert
            var result = await _pressReleaseRepository.FindAsync(c => c.Id == Guid.Parse("692c1e29-d6e7-4ec8-90d5-1b86615e0989"));

            result.ShouldBeNull();
        }
    }
}