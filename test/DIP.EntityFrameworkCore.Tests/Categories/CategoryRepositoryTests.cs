using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using DIP.Categories;
using DIP.EntityFrameworkCore;
using Xunit;

namespace DIP.Categories
{
    public class CategoryRepositoryTests : DIPEntityFrameworkCoreTestBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryRepositoryTests()
        {
            _categoryRepository = GetRequiredService<ICategoryRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _categoryRepository.GetListAsync(
                    titleAr: "1f12efc03f0c4f06af1718ec50935ee8048b136309314d8bbe42bfdf17e691e349075cfa7b584edd8079d6a12d8797c6",
                    titleEn: "6e4d4b10c87a47aeb1357a2aa7d4f5851ab60dadb3724ac",
                    isActive: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("e0b69f5f-79f3-4361-b020-8736ac72984c"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _categoryRepository.GetCountAsync(
                    titleAr: "46f07a97752f487da48cf3c770316b236d6e278ec178411d920cfbb30488c5a8ba09",
                    titleEn: "717baa57f77f40388f95b3673ccfecf4304c9ba6711e49feb31a8ce2328bf635eb45b97",
                    isActive: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}