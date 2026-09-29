using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.EFormServiceSubCategories
{
    public class EFormServiceSubCategoriesAppServiceTests : DIPApplicationTestBase
    {
        private readonly IEFormServiceSubCategoriesAppService _eFormServiceSubCategoriesAppService;
        private readonly IRepository<EFormServiceSubCategory, Guid> _eFormServiceSubCategoryRepository;

        public EFormServiceSubCategoriesAppServiceTests()
        {
            _eFormServiceSubCategoriesAppService = GetRequiredService<IEFormServiceSubCategoriesAppService>();
            _eFormServiceSubCategoryRepository = GetRequiredService<IRepository<EFormServiceSubCategory, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _eFormServiceSubCategoriesAppService.GetListAsync(new GetEFormServiceSubCategoriesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.EFormServiceSubCategory.Id == Guid.Parse("e5dac3a2-de9e-441b-b2c0-e95491e335ba")).ShouldBe(true);
            result.Items.Any(x => x.EFormServiceSubCategory.Id == Guid.Parse("857e2f65-1d71-42e5-99b9-52fc4255bfee")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _eFormServiceSubCategoriesAppService.GetAsync(Guid.Parse("e5dac3a2-de9e-441b-b2c0-e95491e335ba"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("e5dac3a2-de9e-441b-b2c0-e95491e335ba"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new EFormServiceSubCategoryCreateDto
            {
                TitleEn = "d8c3ea4854ce4281991854",
                TitleAr = "cec24995c16742708f6de9cba2b50ce6739edb2261754dd8bd8487c795e8ef08f401481cf",
                File = "b9ee1d4e6",
                Order = 1814233267,
                IsActive = true,
                EFormServiceId = Guid.Parse("f3a4fb45-a8e8-4d7b-9e5f-53cca28817ce")
            };

            // Act
            var serviceResult = await _eFormServiceSubCategoriesAppService.CreateAsync(input);

            // Assert
            var result = await _eFormServiceSubCategoryRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("d8c3ea4854ce4281991854");
            result.TitleAr.ShouldBe("cec24995c16742708f6de9cba2b50ce6739edb2261754dd8bd8487c795e8ef08f401481cf");
            result.File.ShouldBe("b9ee1d4e6");
            result.Order.ShouldBe(1814233267);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new EFormServiceSubCategoryUpdateDto()
            {
                TitleEn = "80ad05ad36b5432190b65bc6f415bcd4346d9807d97f46ab99edb4cfd0cd93d2",
                TitleAr = "8f9e53253ef448e9889bb857855d05808deeb9c0ba334484ba84783740f32a78818da65322c14d5b814",
                File = "11b25620882b4a53b6108b63bb286fda81b6916a5cc54597aa43b17ae06b3b5d591d519643",
                Order = 1150068172,
                IsActive = true,
                EFormServiceId = Guid.Parse("f3a4fb45-a8e8-4d7b-9e5f-53cca28817ce")
            };

            // Act
            var serviceResult = await _eFormServiceSubCategoriesAppService.UpdateAsync(Guid.Parse("e5dac3a2-de9e-441b-b2c0-e95491e335ba"), input);

            // Assert
            var result = await _eFormServiceSubCategoryRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("80ad05ad36b5432190b65bc6f415bcd4346d9807d97f46ab99edb4cfd0cd93d2");
            result.TitleAr.ShouldBe("8f9e53253ef448e9889bb857855d05808deeb9c0ba334484ba84783740f32a78818da65322c14d5b814");
            result.File.ShouldBe("11b25620882b4a53b6108b63bb286fda81b6916a5cc54597aa43b17ae06b3b5d591d519643");
            result.Order.ShouldBe(1150068172);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _eFormServiceSubCategoriesAppService.DeleteAsync(Guid.Parse("e5dac3a2-de9e-441b-b2c0-e95491e335ba"));

            // Assert
            var result = await _eFormServiceSubCategoryRepository.FindAsync(c => c.Id == Guid.Parse("e5dac3a2-de9e-441b-b2c0-e95491e335ba"));

            result.ShouldBeNull();
        }
    }
}