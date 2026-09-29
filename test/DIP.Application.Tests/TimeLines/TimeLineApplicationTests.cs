using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.TimeLines
{
    public class TimeLinesAppServiceTests : DIPApplicationTestBase
    {
        private readonly ITimeLinesAppService _timeLinesAppService;
        private readonly IRepository<TimeLine, Guid> _timeLineRepository;

        public TimeLinesAppServiceTests()
        {
            _timeLinesAppService = GetRequiredService<ITimeLinesAppService>();
            _timeLineRepository = GetRequiredService<IRepository<TimeLine, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _timeLinesAppService.GetListAsync(new GetTimeLinesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.TimeLine.Id == Guid.Parse("b1787d2c-f9c4-4fdc-8fa0-869bb532ab9b")).ShouldBe(true);
            result.Items.Any(x => x.TimeLine.Id == Guid.Parse("34fb95d6-a232-4878-8f04-95a0a3d339ac")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _timeLinesAppService.GetAsync(Guid.Parse("b1787d2c-f9c4-4fdc-8fa0-869bb532ab9b"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("b1787d2c-f9c4-4fdc-8fa0-869bb532ab9b"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new TimeLineCreateDto
            {
                TitleEn = "d920d27549b04d4da9de91b5bd8a5d0d227dbe78f5414dc3b0a1682bd35a568d8635f2d314e447c19e",
                TitleAr = "0e84a26048544cf280c7f9c19331250041f8d8a208e54573a3d361970",
                DescriptionEn = "8f73f64dbcd94817847a631baf719290382b8d714",
                DescriptionAr = "d98edf0ff89b41a193",
                Image = "f8eb4429461b45a8b818a8f8737fc7d69744b28f07a6461594166bd652dcee131d03399b51db4812a3",
                TimeLineDate = new DateTime(2020, 9, 24),
                Order = 1023350191,
                IsActive = true,
                TimeLineCategoryId = Guid.Parse("b5a456d9-416a-4464-a807-1dc3f46c7440")
            };

            // Act
            var serviceResult = await _timeLinesAppService.CreateAsync(input);

            // Assert
            var result = await _timeLineRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("d920d27549b04d4da9de91b5bd8a5d0d227dbe78f5414dc3b0a1682bd35a568d8635f2d314e447c19e");
            result.TitleAr.ShouldBe("0e84a26048544cf280c7f9c19331250041f8d8a208e54573a3d361970");
            result.DescriptionEn.ShouldBe("8f73f64dbcd94817847a631baf719290382b8d714");
            result.DescriptionAr.ShouldBe("d98edf0ff89b41a193");
            result.Image.ShouldBe("f8eb4429461b45a8b818a8f8737fc7d69744b28f07a6461594166bd652dcee131d03399b51db4812a3");
            result.TimeLineDate.ShouldBe(new DateTime(2020, 9, 24));
            result.Order.ShouldBe(1023350191);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new TimeLineUpdateDto()
            {
                TitleEn = "5edf6a0407fa40658aaef9c9a000fd4d18eca111fb1b42dc98",
                TitleAr = "b2b7dac2f6484edfadfb93",
                DescriptionEn = "922842fe4379427fa41e78ec0aa",
                DescriptionAr = "04767a116a744b63a297c9be72c6f62f73eacc8c1de34bb48dba9774ccc72287475a48955e9b4f44ac7f7142b48d72",
                Image = "1d638d436db548c78308b1e8dce9b955399206fa1fff46d3a27a73f0980fa068ae023984fb5b4a05a128532cd4",
                TimeLineDate = new DateTime(2003, 2, 2),
                Order = 1828947249,
                IsActive = true,
                TimeLineCategoryId = Guid.Parse("b5a456d9-416a-4464-a807-1dc3f46c7440")
            };

            // Act
            var serviceResult = await _timeLinesAppService.UpdateAsync(Guid.Parse("b1787d2c-f9c4-4fdc-8fa0-869bb532ab9b"), input);

            // Assert
            var result = await _timeLineRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("5edf6a0407fa40658aaef9c9a000fd4d18eca111fb1b42dc98");
            result.TitleAr.ShouldBe("b2b7dac2f6484edfadfb93");
            result.DescriptionEn.ShouldBe("922842fe4379427fa41e78ec0aa");
            result.DescriptionAr.ShouldBe("04767a116a744b63a297c9be72c6f62f73eacc8c1de34bb48dba9774ccc72287475a48955e9b4f44ac7f7142b48d72");
            result.Image.ShouldBe("1d638d436db548c78308b1e8dce9b955399206fa1fff46d3a27a73f0980fa068ae023984fb5b4a05a128532cd4");
            result.TimeLineDate.ShouldBe(new DateTime(2003, 2, 2));
            result.Order.ShouldBe(1828947249);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _timeLinesAppService.DeleteAsync(Guid.Parse("b1787d2c-f9c4-4fdc-8fa0-869bb532ab9b"));

            // Assert
            var result = await _timeLineRepository.FindAsync(c => c.Id == Guid.Parse("b1787d2c-f9c4-4fdc-8fa0-869bb532ab9b"));

            result.ShouldBeNull();
        }
    }
}