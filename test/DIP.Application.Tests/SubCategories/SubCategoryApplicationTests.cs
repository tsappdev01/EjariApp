using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.SubCategories
{
    public class SubCategoriesAppServiceTests : DIPApplicationTestBase
    {
        private readonly ISubCategoriesAppService _subCategoriesAppService;
        private readonly IRepository<SubCategory, Guid> _subCategoryRepository;

        public SubCategoriesAppServiceTests()
        {
            _subCategoriesAppService = GetRequiredService<ISubCategoriesAppService>();
            _subCategoryRepository = GetRequiredService<IRepository<SubCategory, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _subCategoriesAppService.GetListAsync(new GetSubCategoriesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.SubCategory.Id == Guid.Parse("8d83126a-a8e9-4b30-99a4-bef92dfd31a7")).ShouldBe(true);
            result.Items.Any(x => x.SubCategory.Id == Guid.Parse("19c4fe77-b7f7-4426-92ab-28c56a19c9c5")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _subCategoriesAppService.GetAsync(Guid.Parse("8d83126a-a8e9-4b30-99a4-bef92dfd31a7"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("8d83126a-a8e9-4b30-99a4-bef92dfd31a7"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new SubCategoryCreateDto
            {
                TitleAr = "3c1ea8f5aabd4a468d8caa482bc58f4478a7f62",
                TitleEn = "e808d895db62442daf6d160fff3ed",
                Order = 2022280607,
                IsActive = true,
                CategoryId = Guid.Parse("059ae14e-2021-4827-b59e-f648a17a3c0d")
            };

            // Act
            var serviceResult = await _subCategoriesAppService.CreateAsync(input);

            // Assert
            var result = await _subCategoryRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleAr.ShouldBe("3c1ea8f5aabd4a468d8caa482bc58f4478a7f62");
            result.TitleEn.ShouldBe("e808d895db62442daf6d160fff3ed");
            result.Order.ShouldBe(2022280607);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new SubCategoryUpdateDto()
            {
                TitleAr = "61875d0526ff419d83034f0f2f582166",
                TitleEn = "60776d65a44e49fba7d3f807d8df91dbe5763720409641249785c0c",
                Order = 303086302,
                IsActive = true,
                CategoryId = Guid.Parse("059ae14e-2021-4827-b59e-f648a17a3c0d")
            };

            // Act
            var serviceResult = await _subCategoriesAppService.UpdateAsync(Guid.Parse("8d83126a-a8e9-4b30-99a4-bef92dfd31a7"), input);

            // Assert
            var result = await _subCategoryRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleAr.ShouldBe("61875d0526ff419d83034f0f2f582166");
            result.TitleEn.ShouldBe("60776d65a44e49fba7d3f807d8df91dbe5763720409641249785c0c");
            result.Order.ShouldBe(303086302);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _subCategoriesAppService.DeleteAsync(Guid.Parse("8d83126a-a8e9-4b30-99a4-bef92dfd31a7"));

            // Assert
            var result = await _subCategoryRepository.FindAsync(c => c.Id == Guid.Parse("8d83126a-a8e9-4b30-99a4-bef92dfd31a7"));

            result.ShouldBeNull();
        }
    }
}