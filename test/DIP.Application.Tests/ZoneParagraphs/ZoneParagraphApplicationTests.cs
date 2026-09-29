using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.ZoneParagraphs
{
    public class ZoneParagraphsAppServiceTests : DIPApplicationTestBase
    {
        private readonly IZoneParagraphsAppService _zoneParagraphsAppService;
        private readonly IRepository<ZoneParagraph, Guid> _zoneParagraphRepository;

        public ZoneParagraphsAppServiceTests()
        {
            _zoneParagraphsAppService = GetRequiredService<IZoneParagraphsAppService>();
            _zoneParagraphRepository = GetRequiredService<IRepository<ZoneParagraph, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _zoneParagraphsAppService.GetListAsync(new GetZoneParagraphsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.ZoneParagraph.Id == Guid.Parse("a289ab56-f9c9-476d-aed3-4fb91d7d14be")).ShouldBe(true);
            result.Items.Any(x => x.ZoneParagraph.Id == Guid.Parse("eba7ff10-b3bd-495a-b338-0ab253e78f8a")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _zoneParagraphsAppService.GetAsync(Guid.Parse("a289ab56-f9c9-476d-aed3-4fb91d7d14be"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("a289ab56-f9c9-476d-aed3-4fb91d7d14be"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new ZoneParagraphCreateDto
            {
                TitleEn = "d0c99fbfc17a44fdbe66bbcd27f6b8",
                TitleAr = "0bd2203e535d4dc9b9c406a5441363456c22a81e90d348bb86dd07d345af3e7b",
                SubTilteEn = "7230323668334a28ac50e1aa5328687bc6e9c724fbfa4644b985dbb27d2679af4077f0bf609c437883504d09",
                SubTitleAr = "5aec37aa45894df7abc325ae6bdf3e9b92e4f785b9504e47bc8ef6f4",
                DescriptionEn = "2912d8b91012433daf8eb2042349e22d3e50261eaf",
                DescriptionAr = "9e2bcaf5d3494cf19f4f20c3c498d003d23a2bbf98e04dd18da81bd77376a9710e1783c5899d41258d",
                Order = 1073225044,
                IsActive = true,
                ZoneId = Guid.Parse("5968a2a9-f7fc-469f-904c-3f176982876e")
            };

            // Act
            var serviceResult = await _zoneParagraphsAppService.CreateAsync(input);

            // Assert
            var result = await _zoneParagraphRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("d0c99fbfc17a44fdbe66bbcd27f6b8");
            result.TitleAr.ShouldBe("0bd2203e535d4dc9b9c406a5441363456c22a81e90d348bb86dd07d345af3e7b");
            result.SubTilteEn.ShouldBe("7230323668334a28ac50e1aa5328687bc6e9c724fbfa4644b985dbb27d2679af4077f0bf609c437883504d09");
            result.SubTitleAr.ShouldBe("5aec37aa45894df7abc325ae6bdf3e9b92e4f785b9504e47bc8ef6f4");
            result.DescriptionEn.ShouldBe("2912d8b91012433daf8eb2042349e22d3e50261eaf");
            result.DescriptionAr.ShouldBe("9e2bcaf5d3494cf19f4f20c3c498d003d23a2bbf98e04dd18da81bd77376a9710e1783c5899d41258d");
            result.Order.ShouldBe(1073225044);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new ZoneParagraphUpdateDto()
            {
                TitleEn = "6f3ab1613ede477c8fb5df5cc4515bb866962c78ec384",
                TitleAr = "8bfd29558e6b4a3aa84f175175e74101c15c53756e214fc7a5a277833b9198fa77178b991b9141d",
                SubTilteEn = "1e49c64f585247dea5001863459fb15af58061722cb94f87a1c166",
                SubTitleAr = "3f1b56ed287a4b20a4a945cd5a6ae7c298cb6a87d4034c0f8ed9fdc0",
                DescriptionEn = "bb5e64a33b4643caa52f00a7f9b35fda438048272d5e44e6ba67a4f30d9c86b6fceb9c3b",
                DescriptionAr = "a8541dc10b2f49ebb556c637139836d8040ca98e23f74c1c9108000f6482d01b063a4e",
                Order = 193041805,
                IsActive = true,
                ZoneId = Guid.Parse("5968a2a9-f7fc-469f-904c-3f176982876e")
            };

            // Act
            var serviceResult = await _zoneParagraphsAppService.UpdateAsync(Guid.Parse("a289ab56-f9c9-476d-aed3-4fb91d7d14be"), input);

            // Assert
            var result = await _zoneParagraphRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("6f3ab1613ede477c8fb5df5cc4515bb866962c78ec384");
            result.TitleAr.ShouldBe("8bfd29558e6b4a3aa84f175175e74101c15c53756e214fc7a5a277833b9198fa77178b991b9141d");
            result.SubTilteEn.ShouldBe("1e49c64f585247dea5001863459fb15af58061722cb94f87a1c166");
            result.SubTitleAr.ShouldBe("3f1b56ed287a4b20a4a945cd5a6ae7c298cb6a87d4034c0f8ed9fdc0");
            result.DescriptionEn.ShouldBe("bb5e64a33b4643caa52f00a7f9b35fda438048272d5e44e6ba67a4f30d9c86b6fceb9c3b");
            result.DescriptionAr.ShouldBe("a8541dc10b2f49ebb556c637139836d8040ca98e23f74c1c9108000f6482d01b063a4e");
            result.Order.ShouldBe(193041805);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _zoneParagraphsAppService.DeleteAsync(Guid.Parse("a289ab56-f9c9-476d-aed3-4fb91d7d14be"));

            // Assert
            var result = await _zoneParagraphRepository.FindAsync(c => c.Id == Guid.Parse("a289ab56-f9c9-476d-aed3-4fb91d7d14be"));

            result.ShouldBeNull();
        }
    }
}