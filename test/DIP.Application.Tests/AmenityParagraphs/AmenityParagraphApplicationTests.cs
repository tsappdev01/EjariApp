using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.AmenityParagraphs
{
    public class AmenityParagraphsAppServiceTests : DIPApplicationTestBase
    {
        private readonly IAmenityParagraphsAppService _amenityParagraphsAppService;
        private readonly IRepository<AmenityParagraph, Guid> _amenityParagraphRepository;

        public AmenityParagraphsAppServiceTests()
        {
            _amenityParagraphsAppService = GetRequiredService<IAmenityParagraphsAppService>();
            _amenityParagraphRepository = GetRequiredService<IRepository<AmenityParagraph, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _amenityParagraphsAppService.GetListAsync(new GetAmenityParagraphsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.AmenityParagraph.Id == Guid.Parse("e7f94fcc-8906-4dad-a3ce-045e04bc2bf4")).ShouldBe(true);
            result.Items.Any(x => x.AmenityParagraph.Id == Guid.Parse("af060edf-39ed-4f40-a12d-719789c3484d")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _amenityParagraphsAppService.GetAsync(Guid.Parse("e7f94fcc-8906-4dad-a3ce-045e04bc2bf4"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("e7f94fcc-8906-4dad-a3ce-045e04bc2bf4"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new AmenityParagraphCreateDto
            {
                TitleEn = "5d2605f8399c4343b7bc4f784d66b5b5252f59e5b0684d599e6555c770e6267972e27ce377324ed1aa6dbc788",
                TitleAr = "7494a3ba6cec4d3fad1faf3a64a575f6b655bc8024c446bdb09c32039c6ceb2919f556e99b6c491dac80f44bb9",
                SubTitleEn = "9ad647e7daf74e6c8b9293b64388f29548450b780298465f907792b69a490fdf4d191c795dbd4b7280",
                SubTitleAr = "35f0943b80ed4b0cbade6ee23ba05f029afd36be114c458a8a80ccd92916540dc7de7509a6834",
                DescriptionEn = "652b37ce4f4e4089870b54b37fdb481b32279079c0ac44799c34b4df1cee",
                DescriptionAr = "96e1ce0f543d403d89c1e2dbdda51a0ee492751",
                ButtonUrl = "cfbfa4948dc14206",
                Order = 365443252,
                IsActive = true,
                AmenityId = Guid.Parse("d2da12fa-8dc8-4585-a9b2-1e074fde2f9b")
            };

            // Act
            var serviceResult = await _amenityParagraphsAppService.CreateAsync(input);

            // Assert
            var result = await _amenityParagraphRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("5d2605f8399c4343b7bc4f784d66b5b5252f59e5b0684d599e6555c770e6267972e27ce377324ed1aa6dbc788");
            result.TitleAr.ShouldBe("7494a3ba6cec4d3fad1faf3a64a575f6b655bc8024c446bdb09c32039c6ceb2919f556e99b6c491dac80f44bb9");
            result.SubTitleEn.ShouldBe("9ad647e7daf74e6c8b9293b64388f29548450b780298465f907792b69a490fdf4d191c795dbd4b7280");
            result.SubTitleAr.ShouldBe("35f0943b80ed4b0cbade6ee23ba05f029afd36be114c458a8a80ccd92916540dc7de7509a6834");
            result.DescriptionEn.ShouldBe("652b37ce4f4e4089870b54b37fdb481b32279079c0ac44799c34b4df1cee");
            result.DescriptionAr.ShouldBe("96e1ce0f543d403d89c1e2dbdda51a0ee492751");
            result.ButtonUrl.ShouldBe("cfbfa4948dc14206");
            result.Order.ShouldBe(365443252);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new AmenityParagraphUpdateDto()
            {
                TitleEn = "e5beae1faac849baa21623087fe9367facbc575f5c3b4a829690b5e4d547e",
                TitleAr = "6008550a99d840c4a29d07c33af3c80d4e3fc4bad3854a9c99",
                SubTitleEn = "2589eee4ca15429ba377b66ff91ce5",
                SubTitleAr = "a280e200289747578b82176208411156df769dec18154541865fd22395d07a",
                DescriptionEn = "94f9b5e",
                DescriptionAr = "c1b76a7efa294c75b014975112671987eb66dbf4decc4804a781746fc3a0",
                ButtonUrl = "bbfcacc65e474220a15dd092645958d16bc65553b3214425b895ad72204",
                Order = 1367673443,
                IsActive = true,
                AmenityId = Guid.Parse("d2da12fa-8dc8-4585-a9b2-1e074fde2f9b")
            };

            // Act
            var serviceResult = await _amenityParagraphsAppService.UpdateAsync(Guid.Parse("e7f94fcc-8906-4dad-a3ce-045e04bc2bf4"), input);

            // Assert
            var result = await _amenityParagraphRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("e5beae1faac849baa21623087fe9367facbc575f5c3b4a829690b5e4d547e");
            result.TitleAr.ShouldBe("6008550a99d840c4a29d07c33af3c80d4e3fc4bad3854a9c99");
            result.SubTitleEn.ShouldBe("2589eee4ca15429ba377b66ff91ce5");
            result.SubTitleAr.ShouldBe("a280e200289747578b82176208411156df769dec18154541865fd22395d07a");
            result.DescriptionEn.ShouldBe("94f9b5e");
            result.DescriptionAr.ShouldBe("c1b76a7efa294c75b014975112671987eb66dbf4decc4804a781746fc3a0");
            result.ButtonUrl.ShouldBe("bbfcacc65e474220a15dd092645958d16bc65553b3214425b895ad72204");
            result.Order.ShouldBe(1367673443);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _amenityParagraphsAppService.DeleteAsync(Guid.Parse("e7f94fcc-8906-4dad-a3ce-045e04bc2bf4"));

            // Assert
            var result = await _amenityParagraphRepository.FindAsync(c => c.Id == Guid.Parse("e7f94fcc-8906-4dad-a3ce-045e04bc2bf4"));

            result.ShouldBeNull();
        }
    }
}