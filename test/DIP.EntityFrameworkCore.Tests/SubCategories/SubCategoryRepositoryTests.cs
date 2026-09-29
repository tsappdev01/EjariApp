using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using DIP.SubCategories;
using DIP.EntityFrameworkCore;
using Xunit;

namespace DIP.SubCategories
{
    public class SubCategoryRepositoryTests : DIPEntityFrameworkCoreTestBase
    {
        private readonly ISubCategoryRepository _subCategoryRepository;

        public SubCategoryRepositoryTests()
        {
            _subCategoryRepository = GetRequiredService<ISubCategoryRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _subCategoryRepository.GetListAsync(
                    titleAr: "ce1fade4bd8c46d68557700aefc1a94c6e262373e36a4059b059d",
                    titleEn: "4500fb4b40a6407ca9de8722754c6b8fa200658252bb45729e364f7196e79602329d45622f7d497",
                    isActive: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("8d83126a-a8e9-4b30-99a4-bef92dfd31a7"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _subCategoryRepository.GetCountAsync(
                    titleAr: "16f6e6112a57484683d47dd4",
                    titleEn: "da117cf50ff04479bb3f8da7a6d3eb1191fb3efef010454ab6caddade45ba3a9b1b25e4",
                    isActive: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}