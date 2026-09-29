using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.Medias
{
    public class MediasAppServiceTests : DIPApplicationTestBase
    {
        private readonly IMediasAppService _mediasAppService;
        private readonly IRepository<Media, Guid> _mediaRepository;

        public MediasAppServiceTests()
        {
            _mediasAppService = GetRequiredService<IMediasAppService>();
            _mediaRepository = GetRequiredService<IRepository<Media, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _mediasAppService.GetListAsync(new GetMediasInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Media.Id == Guid.Parse("4a7b9266-2cd3-4bbb-bf62-8043e8eb54ed")).ShouldBe(true);
            result.Items.Any(x => x.Media.Id == Guid.Parse("77639ba9-cba5-46a1-9605-0eda1b8a4500")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _mediasAppService.GetAsync(Guid.Parse("4a7b9266-2cd3-4bbb-bf62-8043e8eb54ed"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("4a7b9266-2cd3-4bbb-bf62-8043e8eb54ed"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new MediaCreateDto
            {
                TitleEn = "b3981b27ab1b49728c177acde9f83057f028c063fb704aa083d",
                TitleAr = "902cc3838c7a42b1878ddb759e1223b09e048821b6754a869cdf30ff4e4256bae8b39ce06df44ababcd81fd277a33a9ea9",
                File = "6370a2f026414da",
                Order = 1641434302,
                IsActive = true
            };

            // Act
            var serviceResult = await _mediasAppService.CreateAsync(input);

            // Assert
            var result = await _mediaRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("b3981b27ab1b49728c177acde9f83057f028c063fb704aa083d");
            result.TitleAr.ShouldBe("902cc3838c7a42b1878ddb759e1223b09e048821b6754a869cdf30ff4e4256bae8b39ce06df44ababcd81fd277a33a9ea9");
            result.File.ShouldBe("6370a2f026414da");
            result.Order.ShouldBe(1641434302);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new MediaUpdateDto()
            {
                TitleEn = "c5a1e808f6364c928d3260a7e177f34d9c29773c02524fb38b37938d74d75a6cc2ec085277464149",
                TitleAr = "9913cb0c334d4f7d9637003ca347dd357f06ffa9a6d14cf3994facab89c0312a50b5f6449791",
                File = "0d8561ca8d2446a199640b00",
                Order = 1389333559,
                IsActive = true
            };

            // Act
            var serviceResult = await _mediasAppService.UpdateAsync(Guid.Parse("4a7b9266-2cd3-4bbb-bf62-8043e8eb54ed"), input);

            // Assert
            var result = await _mediaRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("c5a1e808f6364c928d3260a7e177f34d9c29773c02524fb38b37938d74d75a6cc2ec085277464149");
            result.TitleAr.ShouldBe("9913cb0c334d4f7d9637003ca347dd357f06ffa9a6d14cf3994facab89c0312a50b5f6449791");
            result.File.ShouldBe("0d8561ca8d2446a199640b00");
            result.Order.ShouldBe(1389333559);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _mediasAppService.DeleteAsync(Guid.Parse("4a7b9266-2cd3-4bbb-bf62-8043e8eb54ed"));

            // Assert
            var result = await _mediaRepository.FindAsync(c => c.Id == Guid.Parse("4a7b9266-2cd3-4bbb-bf62-8043e8eb54ed"));

            result.ShouldBeNull();
        }
    }
}