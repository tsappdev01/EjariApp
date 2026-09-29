using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.Categories
{
    public class CategoriesAppServiceTests : DIPApplicationTestBase
    {
        private readonly ICategoriesAppService _categoriesAppService;
        private readonly IRepository<Category, Guid> _categoryRepository;

        public CategoriesAppServiceTests()
        {
            _categoriesAppService = GetRequiredService<ICategoriesAppService>();
            _categoryRepository = GetRequiredService<IRepository<Category, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _categoriesAppService.GetListAsync(new GetCategoriesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("e0b69f5f-79f3-4361-b020-8736ac72984c")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("d199f211-8c24-409c-8a60-bc272b1ad3a3")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _categoriesAppService.GetAsync(Guid.Parse("e0b69f5f-79f3-4361-b020-8736ac72984c"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("e0b69f5f-79f3-4361-b020-8736ac72984c"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new CategoryCreateDto
            {
                TitleAr = "e621ee21405",
                TitleEn = "3580906292904f4fba1457a51",
                Order = 16937047,
                IsActive = true
            };

            // Act
            var serviceResult = await _categoriesAppService.CreateAsync(input);

            // Assert
            var result = await _categoryRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleAr.ShouldBe("e621ee21405");
            result.TitleEn.ShouldBe("3580906292904f4fba1457a51");
            result.Order.ShouldBe(16937047);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new CategoryUpdateDto()
            {
                TitleAr = "5d294aab2c9d4b79",
                TitleEn = "4e45802824dd4a23ae4a97753807c5d4aa44349592b4403d9473be76b51f461321042f6cf13f4809ab77f7ae5bf5ae",
                Order = 698154555,
                IsActive = true
            };

            // Act
            var serviceResult = await _categoriesAppService.UpdateAsync(Guid.Parse("e0b69f5f-79f3-4361-b020-8736ac72984c"), input);

            // Assert
            var result = await _categoryRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleAr.ShouldBe("5d294aab2c9d4b79");
            result.TitleEn.ShouldBe("4e45802824dd4a23ae4a97753807c5d4aa44349592b4403d9473be76b51f461321042f6cf13f4809ab77f7ae5bf5ae");
            result.Order.ShouldBe(698154555);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _categoriesAppService.DeleteAsync(Guid.Parse("e0b69f5f-79f3-4361-b020-8736ac72984c"));

            // Assert
            var result = await _categoryRepository.FindAsync(c => c.Id == Guid.Parse("e0b69f5f-79f3-4361-b020-8736ac72984c"));

            result.ShouldBeNull();
        }
    }
}