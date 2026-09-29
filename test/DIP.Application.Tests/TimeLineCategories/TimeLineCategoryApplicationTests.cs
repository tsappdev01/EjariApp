using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.TimeLineCategories
{
    public class TimeLineCategoriesAppServiceTests : DIPApplicationTestBase
    {
        private readonly ITimeLineCategoriesAppService _timeLineCategoriesAppService;
        private readonly IRepository<TimeLineCategory, Guid> _timeLineCategoryRepository;

        public TimeLineCategoriesAppServiceTests()
        {
            _timeLineCategoriesAppService = GetRequiredService<ITimeLineCategoriesAppService>();
            _timeLineCategoryRepository = GetRequiredService<IRepository<TimeLineCategory, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _timeLineCategoriesAppService.GetListAsync(new GetTimeLineCategoriesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("b5a456d9-416a-4464-a807-1dc3f46c7440")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("e86fe1ad-fc20-405d-b37a-8ccf64d26436")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _timeLineCategoriesAppService.GetAsync(Guid.Parse("b5a456d9-416a-4464-a807-1dc3f46c7440"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("b5a456d9-416a-4464-a807-1dc3f46c7440"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new TimeLineCategoryCreateDto
            {
                TitleEn = "15c5b6e5e452435b84db6bd80b8e45599e9aa5883a8b4413b55fc8bbc8dd8ff0b8003561f68d4f1",
                TitleAr = "8dad142f4fea4a6ead2eb88d867bb5325b7bea338f0",
                Order = 629644486,
                IsActive = true
            };

            // Act
            var serviceResult = await _timeLineCategoriesAppService.CreateAsync(input);

            // Assert
            var result = await _timeLineCategoryRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("15c5b6e5e452435b84db6bd80b8e45599e9aa5883a8b4413b55fc8bbc8dd8ff0b8003561f68d4f1");
            result.TitleAr.ShouldBe("8dad142f4fea4a6ead2eb88d867bb5325b7bea338f0");
            result.Order.ShouldBe(629644486);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new TimeLineCategoryUpdateDto()
            {
                TitleEn = "40a8779a079d4acda7708d6d14a400f34e52faf4207a4d",
                TitleAr = "db657bfdf13c4e82a196bea7e9e1ee359533e623b9104",
                Order = 875135573,
                IsActive = true
            };

            // Act
            var serviceResult = await _timeLineCategoriesAppService.UpdateAsync(Guid.Parse("b5a456d9-416a-4464-a807-1dc3f46c7440"), input);

            // Assert
            var result = await _timeLineCategoryRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("40a8779a079d4acda7708d6d14a400f34e52faf4207a4d");
            result.TitleAr.ShouldBe("db657bfdf13c4e82a196bea7e9e1ee359533e623b9104");
            result.Order.ShouldBe(875135573);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _timeLineCategoriesAppService.DeleteAsync(Guid.Parse("b5a456d9-416a-4464-a807-1dc3f46c7440"));

            // Assert
            var result = await _timeLineCategoryRepository.FindAsync(c => c.Id == Guid.Parse("b5a456d9-416a-4464-a807-1dc3f46c7440"));

            result.ShouldBeNull();
        }
    }
}