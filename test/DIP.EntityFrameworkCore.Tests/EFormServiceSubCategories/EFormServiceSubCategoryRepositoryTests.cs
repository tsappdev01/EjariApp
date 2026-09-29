using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using DIP.EFormServiceSubCategories;
using DIP.EntityFrameworkCore;
using Xunit;

namespace DIP.EFormServiceSubCategories
{
    public class EFormServiceSubCategoryRepositoryTests : DIPEntityFrameworkCoreTestBase
    {
        private readonly IEFormServiceSubCategoryRepository _eFormServiceSubCategoryRepository;

        public EFormServiceSubCategoryRepositoryTests()
        {
            _eFormServiceSubCategoryRepository = GetRequiredService<IEFormServiceSubCategoryRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _eFormServiceSubCategoryRepository.GetListAsync(
                    titleEn: "fc1b011eb07847b3bf47f7ad902e",
                    titleAr: "4d1eddb802494fc9bcbf21a834e81dcca1010f95d6ed49718425feb7e311d6691591dcfe92ce4bf9ab7906a688365830c33",
                    file: "9893e9ace63a4db780fc6125ab15d053632d91f01e0f42979",
                    isActive: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("e5dac3a2-de9e-441b-b2c0-e95491e335ba"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _eFormServiceSubCategoryRepository.GetCountAsync(
                    titleEn: "ae01a134f3344e3ea3e109e5aa98c077fc238f8c85af4868b63696b940f33b9174e93e6944744a",
                    titleAr: "b999a72c00be465",
                    file: "6ac9b5c7c7ac44c8b",
                    isActive: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}