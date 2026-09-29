using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.Amenities
{
    public class AmenitiesAppServiceTests : DIPApplicationTestBase
    {
        private readonly IAmenitiesAppService _amenitiesAppService;
        private readonly IRepository<Amenity, Guid> _amenityRepository;

        public AmenitiesAppServiceTests()
        {
            _amenitiesAppService = GetRequiredService<IAmenitiesAppService>();
            _amenityRepository = GetRequiredService<IRepository<Amenity, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _amenitiesAppService.GetListAsync(new GetAmenitiesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("81959370-0ccd-4dda-9464-17eaa13ac3a4")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("18fdf9de-266d-44c6-946d-268af285c4f8")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _amenitiesAppService.GetAsync(Guid.Parse("81959370-0ccd-4dda-9464-17eaa13ac3a4"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("81959370-0ccd-4dda-9464-17eaa13ac3a4"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new AmenityCreateDto
            {
                TitleEn = "bd9b6f10c59c436f95",
                TitleAr = "cf4b15429b60419aa39d9f5e870342f4167f818d04cd4e25a4",
                MetaTitleEn = "9604650c7689404b919163c65bd3bdb8a13b37da46054a59a7392bd364ac11891acca6936353474f",
                MetaDescriptionEn = "254ba29cc4ca42aba9b5342d53",
                MetaTitleAr = "a316c88776d34",
                MetaDescriptionAr = "e3d8841e214047358c",
                Slug = "a1fef1ee2c7e40358d94fb841d6be36ed12873bf41194e159027c68fe2f08eff3",
                HeaderImage = "b0add7c7cd654734bf9a614e996037476ef86cd76cf445cbb629130f325ba182bf80d8b6231a4",
                Order = 886011197,
                IsActive = true
            };

            // Act
            var serviceResult = await _amenitiesAppService.CreateAsync(input);

            // Assert
            var result = await _amenityRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("bd9b6f10c59c436f95");
            result.TitleAr.ShouldBe("cf4b15429b60419aa39d9f5e870342f4167f818d04cd4e25a4");
            result.MetaTitleEn.ShouldBe("9604650c7689404b919163c65bd3bdb8a13b37da46054a59a7392bd364ac11891acca6936353474f");
            result.MetaDescriptionEn.ShouldBe("254ba29cc4ca42aba9b5342d53");
            result.MetaTitleAr.ShouldBe("a316c88776d34");
            result.MetaDescriptionAr.ShouldBe("e3d8841e214047358c");
            result.Slug.ShouldBe("a1fef1ee2c7e40358d94fb841d6be36ed12873bf41194e159027c68fe2f08eff3");
            result.HeaderImage.ShouldBe("b0add7c7cd654734bf9a614e996037476ef86cd76cf445cbb629130f325ba182bf80d8b6231a4");
            result.Order.ShouldBe(886011197);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new AmenityUpdateDto()
            {
                TitleEn = "421f120",
                TitleAr = "9c691dc391424f0d8c1695c8c7ec1725542f9ee3fc8548fda34a2c508057d3fe64ca5906",
                MetaTitleEn = "591541d562714a9692a570cadd1433336a5825b5d0744879b8ca11d9819081872fa5ce6ba4704d3",
                MetaDescriptionEn = "81f72e9defd",
                MetaTitleAr = "9d827885ab304f74b7dbb099273986eb913bf48a3",
                MetaDescriptionAr = "f79bcb539d9a41",
                Slug = "d1791865c41d4004a5c6b41893eee932ab35d2c6e5e44565941d739528ea8232512c88e0f10e44e",
                HeaderImage = "01c4fb73f292497",
                Order = 770958973,
                IsActive = true
            };

            // Act
            var serviceResult = await _amenitiesAppService.UpdateAsync(Guid.Parse("81959370-0ccd-4dda-9464-17eaa13ac3a4"), input);

            // Assert
            var result = await _amenityRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("421f120");
            result.TitleAr.ShouldBe("9c691dc391424f0d8c1695c8c7ec1725542f9ee3fc8548fda34a2c508057d3fe64ca5906");
            result.MetaTitleEn.ShouldBe("591541d562714a9692a570cadd1433336a5825b5d0744879b8ca11d9819081872fa5ce6ba4704d3");
            result.MetaDescriptionEn.ShouldBe("81f72e9defd");
            result.MetaTitleAr.ShouldBe("9d827885ab304f74b7dbb099273986eb913bf48a3");
            result.MetaDescriptionAr.ShouldBe("f79bcb539d9a41");
            result.Slug.ShouldBe("d1791865c41d4004a5c6b41893eee932ab35d2c6e5e44565941d739528ea8232512c88e0f10e44e");
            result.HeaderImage.ShouldBe("01c4fb73f292497");
            result.Order.ShouldBe(770958973);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _amenitiesAppService.DeleteAsync(Guid.Parse("81959370-0ccd-4dda-9464-17eaa13ac3a4"));

            // Assert
            var result = await _amenityRepository.FindAsync(c => c.Id == Guid.Parse("81959370-0ccd-4dda-9464-17eaa13ac3a4"));

            result.ShouldBeNull();
        }
    }
}