using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.SliderHomePages
{
    public class SliderHomePagesAppServiceTests : DIPApplicationTestBase
    {
        private readonly ISliderHomePagesAppService _sliderHomePagesAppService;
        private readonly IRepository<SliderHomePage, Guid> _sliderHomePageRepository;

        public SliderHomePagesAppServiceTests()
        {
            _sliderHomePagesAppService = GetRequiredService<ISliderHomePagesAppService>();
            _sliderHomePageRepository = GetRequiredService<IRepository<SliderHomePage, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _sliderHomePagesAppService.GetListAsync(new GetSliderHomePagesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("be8556e4-a7f1-4783-a891-03d4ebe23a73")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("599c48d6-33f6-457e-9220-395b844e732c")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _sliderHomePagesAppService.GetAsync(Guid.Parse("be8556e4-a7f1-4783-a891-03d4ebe23a73"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("be8556e4-a7f1-4783-a891-03d4ebe23a73"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new SliderHomePageCreateDto
            {
                TitleAr = "905289e27e134af9974f01c305c61a1dc3798f520eb84548868cb873ba6009b36",
                TitleEn = "d9360f243822442b98a4a251f7a91fb7ed89592f92514a36aef8016d343a5",
                DescriptionAr = "28e44d42fbe74d3",
                DescriptionEn = "d53597",
                ButtonTitleAr = "edd9a13cb5de482",
                ButtonTitleEn = "752c6885165d43e18cbeb5302c96a1b94b16d738e75144c9b1a3ff493083f336cff3a3750bfd467",
                ButtonUrlEn = "60d09833094c40379e7565290317168c8bc9f914946f43a3ab92193dd3ada0e8a918b827474043979",
                ButtonUrlAr = "60d09833094c40379e7565290317168c8bc9f914946f43a3ab92193dd3ada0e8a918b827474043979",
                Image = "b16e745130d64eb4a57089037e2dd1f8214624c710db48feab391fc60969cc9748da99a3b20244bdafdb9cbb5",
                YoutubeUrl = "b89fc5b3fe9a4b6e9d3",
                IsActive = true,
                Order = 938569868
            };

            // Act
            var serviceResult = await _sliderHomePagesAppService.CreateAsync(input);

            // Assert
            var result = await _sliderHomePageRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleAr.ShouldBe("905289e27e134af9974f01c305c61a1dc3798f520eb84548868cb873ba6009b36");
            result.TitleEn.ShouldBe("d9360f243822442b98a4a251f7a91fb7ed89592f92514a36aef8016d343a5");
            result.DescriptionAr.ShouldBe("28e44d42fbe74d3");
            result.DescriptionEn.ShouldBe("d53597");
            result.ButtonTitleAr.ShouldBe("edd9a13cb5de482");
            result.ButtonTitleEn.ShouldBe("752c6885165d43e18cbeb5302c96a1b94b16d738e75144c9b1a3ff493083f336cff3a3750bfd467");
            result.ButtonUrlEn.ShouldBe("60d09833094c40379e7565290317168c8bc9f914946f43a3ab92193dd3ada0e8a918b827474043979");
            result.ButtonUrlAr.ShouldBe("60d09833094c40379e7565290317168c8bc9f914946f43a3ab92193dd3ada0e8a918b827474043979");
            result.Image.ShouldBe("b16e745130d64eb4a57089037e2dd1f8214624c710db48feab391fc60969cc9748da99a3b20244bdafdb9cbb5");
            result.YoutubeUrl.ShouldBe("b89fc5b3fe9a4b6e9d3");
            result.IsActive.ShouldBe(true);
            result.Order.ShouldBe(938569868);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new SliderHomePageUpdateDto()
            {
                TitleAr = "8b73c12c8f61405aa91182b88a55ba09806c2",
                TitleEn = "bb6f57a2af3e4e94bcedc983f44f5aaa68124f67e90",
                DescriptionAr = "1d3fc5977acf4fb393c13d57d7d9f2f7b6",
                DescriptionEn = "84934acb979d424787f6276ee0e4ad20f0b202e0e0db402cbda9e522416b7",
                ButtonTitleAr = "f07d0630d6a1405abed80506849cb78a7a755d7627bc40fea5175d8cc9e1e7",
                ButtonTitleEn = "05cb630709d1",
                ButtonUrlEn = "bec504bde68f4854a2fc05ab54c50018aab",
                ButtonUrlAr = "bec504bde68f4854a2fc05ab54c50018aab",
                Image = "af05c503d5",
                YoutubeUrl = "396d1921d22f487a8e2d77f",
                IsActive = true,
                Order = 736918325
            };

            // Act
            var serviceResult = await _sliderHomePagesAppService.UpdateAsync(Guid.Parse("be8556e4-a7f1-4783-a891-03d4ebe23a73"), input);

            // Assert
            var result = await _sliderHomePageRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleAr.ShouldBe("8b73c12c8f61405aa91182b88a55ba09806c2");
            result.TitleEn.ShouldBe("bb6f57a2af3e4e94bcedc983f44f5aaa68124f67e90");
            result.DescriptionAr.ShouldBe("1d3fc5977acf4fb393c13d57d7d9f2f7b6");
            result.DescriptionEn.ShouldBe("84934acb979d424787f6276ee0e4ad20f0b202e0e0db402cbda9e522416b7");
            result.ButtonTitleAr.ShouldBe("f07d0630d6a1405abed80506849cb78a7a755d7627bc40fea5175d8cc9e1e7");
            result.ButtonTitleEn.ShouldBe("05cb630709d1");
            result.ButtonUrlEn.ShouldBe("bec504bde68f4854a2fc05ab54c50018aab");
            result.ButtonUrlAr.ShouldBe("bec504bde68f4854a2fc05ab54c50018aab");
            result.Image.ShouldBe("af05c503d5");
            result.YoutubeUrl.ShouldBe("396d1921d22f487a8e2d77f");
            result.IsActive.ShouldBe(true);
            result.Order.ShouldBe(736918325);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _sliderHomePagesAppService.DeleteAsync(Guid.Parse("be8556e4-a7f1-4783-a891-03d4ebe23a73"));

            // Assert
            var result = await _sliderHomePageRepository.FindAsync(c => c.Id == Guid.Parse("be8556e4-a7f1-4783-a891-03d4ebe23a73"));

            result.ShouldBeNull();
        }
    }
}