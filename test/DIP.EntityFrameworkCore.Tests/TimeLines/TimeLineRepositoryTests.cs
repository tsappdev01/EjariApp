using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using DIP.TimeLines;
using DIP.EntityFrameworkCore;
using Xunit;

namespace DIP.TimeLines
{
    public class TimeLineRepositoryTests : DIPEntityFrameworkCoreTestBase
    {
        private readonly ITimeLineRepository _timeLineRepository;

        public TimeLineRepositoryTests()
        {
            _timeLineRepository = GetRequiredService<ITimeLineRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _timeLineRepository.GetListAsync(
                    titleEn: "4340b3ad5",
                    titleAr: "e3fb30a74e724d699",
                    descriptionEn: "d0bd60",
                    descriptionAr: "ea0544ded94349e1be115af202428c32861f289c45254954bbec1",
                    image: "cf0943ee90e04b719fe6524f9ea24abb613295850ccd416dbe260de9c241eaae51eecebf6a094cd8a246e",
                    isActive: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("b1787d2c-f9c4-4fdc-8fa0-869bb532ab9b"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _timeLineRepository.GetCountAsync(
                    titleEn: "69bed59410e54c5baf85fcf2d8bd",
                    titleAr: "23479034a5144122b3b23e3a890c51cbec05b1be933a477186ac96358224a4e7b23b6f43b8684ebd98b293",
                    descriptionEn: "a78abfe977a941378",
                    descriptionAr: "e63f6bbd8e3b4958a99f644dba20bbd28",
                    image: "b5c5ca34dd8647c79f358145f72fd1285b",
                    isActive: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}