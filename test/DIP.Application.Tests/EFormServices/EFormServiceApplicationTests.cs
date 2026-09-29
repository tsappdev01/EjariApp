using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.EFormServices
{
    public class EFormServicesAppServiceTests : DIPApplicationTestBase
    {
        private readonly IEFormServicesAppService _eFormServicesAppService;
        private readonly IRepository<EFormService, Guid> _eFormServiceRepository;

        public EFormServicesAppServiceTests()
        {
            _eFormServicesAppService = GetRequiredService<IEFormServicesAppService>();
            _eFormServiceRepository = GetRequiredService<IRepository<EFormService, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _eFormServicesAppService.GetListAsync(new GetEFormServicesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("f3a4fb45-a8e8-4d7b-9e5f-53cca28817ce")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("dce0d74c-2d43-49c0-a63e-5450bcefbf99")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _eFormServicesAppService.GetAsync(Guid.Parse("f3a4fb45-a8e8-4d7b-9e5f-53cca28817ce"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("f3a4fb45-a8e8-4d7b-9e5f-53cca28817ce"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new EFormServiceCreateDto
            {
                TitleEn = "cb6aebe46ef44b8da3bbe8ed72b",
                TitleAr = "949e512656cc48838def6588f62e3d68cd5027c14a494288838c035892a8bc81c3ded6911e9d402fafff18c",
                Order = 344201723,
                IsActive = true
            };

            // Act
            var serviceResult = await _eFormServicesAppService.CreateAsync(input);

            // Assert
            var result = await _eFormServiceRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("cb6aebe46ef44b8da3bbe8ed72b");
            result.TitleAr.ShouldBe("949e512656cc48838def6588f62e3d68cd5027c14a494288838c035892a8bc81c3ded6911e9d402fafff18c");
            result.Order.ShouldBe(344201723);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new EFormServiceUpdateDto()
            {
                TitleEn = "f3c9a5171d3c43da94e9c506ec7cbde8decffc014b5845089bbb39",
                TitleAr = "9ac4fa7855454e71b26feb5fe5bc0c52c45dd1f17a8d4b42949",
                Order = 1554557517,
                IsActive = true
            };

            // Act
            var serviceResult = await _eFormServicesAppService.UpdateAsync(Guid.Parse("f3a4fb45-a8e8-4d7b-9e5f-53cca28817ce"), input);

            // Assert
            var result = await _eFormServiceRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("f3c9a5171d3c43da94e9c506ec7cbde8decffc014b5845089bbb39");
            result.TitleAr.ShouldBe("9ac4fa7855454e71b26feb5fe5bc0c52c45dd1f17a8d4b42949");
            result.Order.ShouldBe(1554557517);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _eFormServicesAppService.DeleteAsync(Guid.Parse("f3a4fb45-a8e8-4d7b-9e5f-53cca28817ce"));

            // Assert
            var result = await _eFormServiceRepository.FindAsync(c => c.Id == Guid.Parse("f3a4fb45-a8e8-4d7b-9e5f-53cca28817ce"));

            result.ShouldBeNull();
        }
    }
}