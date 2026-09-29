using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using DIP.TimeLineCategories;
using DIP.EntityFrameworkCore;
using Xunit;

namespace DIP.TimeLineCategories
{
    public class TimeLineCategoryRepositoryTests : DIPEntityFrameworkCoreTestBase
    {
        private readonly ITimeLineCategoryRepository _timeLineCategoryRepository;

        public TimeLineCategoryRepositoryTests()
        {
            _timeLineCategoryRepository = GetRequiredService<ITimeLineCategoryRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _timeLineCategoryRepository.GetListAsync(
                    titleEn: "15abe51edc0742d2af7fabbcc6c9ecd8284758f9369d4ab0b84b",
                    titleAr: "235291c54c7947d1aca1656275ed40141a31518bf0204b439",
                    isActive: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("b5a456d9-416a-4464-a807-1dc3f46c7440"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _timeLineCategoryRepository.GetCountAsync(
                    titleEn: "f614097e5db045e2af",
                    titleAr: "9291e49819124c17bd86f3e3336e572499554",
                    isActive: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}