using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.MediaGalleries
{
    public class MediaGalleriesAppServiceTests : DIPApplicationTestBase
    {
        private readonly IMediaGalleriesAppService _mediaGalleriesAppService;
        private readonly IRepository<MediaGallery, Guid> _mediaGalleryRepository;

        public MediaGalleriesAppServiceTests()
        {
            _mediaGalleriesAppService = GetRequiredService<IMediaGalleriesAppService>();
            _mediaGalleryRepository = GetRequiredService<IRepository<MediaGallery, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _mediaGalleriesAppService.GetListAsync(new GetMediaGalleriesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("22145e05-3258-4612-91df-358782e69b85")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("9aa50cb3-8d26-47de-9773-17b12f716eea")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _mediaGalleriesAppService.GetAsync(Guid.Parse("22145e05-3258-4612-91df-358782e69b85"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("22145e05-3258-4612-91df-358782e69b85"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new MediaGalleryCreateDto
            {
                TitleEn = "76c68014d0474209a0ef4f683c",
                Slug = "a37e1d4785f1430083f23d26bd56094a3abf35910",
                TitleAr = "7c2366",
                MetaTitleEn = "cbe0cbe2269f4e3eb20b726d1f4da4e1138ec5c8cae348568525db3274",
                MetaTitleAr = "df9d969259864a318c775a74180",
                MetaDescriptionEn = "03831d49583a4093badff0c182098f83e1ff684cb6924d5c8e54f9ec1e036",
                MetaDescriptionAr = "d747b0a4b19443d0bb9b35dc4f9c67a25e7c0ea50f",
                SummaryEn = "31bff770ed6f4c2c9296aba2db9caa77470e8b94c33e472fbcec03d9f7f7c5e6b58a6",
                SummaryAr = "046dfa200d2149a5a8981e0b66a4fd",
                HeaderImage = "e772da54c33d428180142c64d0",
                Image = "59eda5a2cfbf4eeaa4683603bdbb460a060aafa0beac47a894e5a3cb9608af6c0a7ca4481ecc411e934",
                Order = 1808072306,
                IsActive = true
            };

            // Act
            var serviceResult = await _mediaGalleriesAppService.CreateAsync(input);

            // Assert
            var result = await _mediaGalleryRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("76c68014d0474209a0ef4f683c");
            result.Slug.ShouldBe("a37e1d4785f1430083f23d26bd56094a3abf35910");
            result.TitleAr.ShouldBe("7c2366");
            result.MetaTitleEn.ShouldBe("cbe0cbe2269f4e3eb20b726d1f4da4e1138ec5c8cae348568525db3274");
            result.MetaTitleAr.ShouldBe("df9d969259864a318c775a74180");
            result.MetaDescriptionEn.ShouldBe("03831d49583a4093badff0c182098f83e1ff684cb6924d5c8e54f9ec1e036");
            result.MetaDescriptionAr.ShouldBe("d747b0a4b19443d0bb9b35dc4f9c67a25e7c0ea50f");
            result.SummaryEn.ShouldBe("31bff770ed6f4c2c9296aba2db9caa77470e8b94c33e472fbcec03d9f7f7c5e6b58a6");
            result.SummaryAr.ShouldBe("046dfa200d2149a5a8981e0b66a4fd");
            result.HeaderImage.ShouldBe("e772da54c33d428180142c64d0");
            result.Image.ShouldBe("59eda5a2cfbf4eeaa4683603bdbb460a060aafa0beac47a894e5a3cb9608af6c0a7ca4481ecc411e934");
            result.Order.ShouldBe(1808072306);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new MediaGalleryUpdateDto()
            {
                TitleEn = "ed37bb2dd29048db816872d8ddec7",
                Slug = "9c290c46a7ab4b6ca87548b177",
                TitleAr = "f84b906413d949628b64bb72bc4",
                MetaTitleEn = "d357c827f79c440c89883bdd4475d77dff3b",
                MetaTitleAr = "b70eae92e84a43bd9",
                MetaDescriptionEn = "7873fa343f574906b26111015478c94d3844009a4efa4ed4b121",
                MetaDescriptionAr = "a1d22c1e",
                SummaryEn = "8d9ec613f4444c039cdc0d2a85ad66769c318960728b40ce92e0187512a2c0f6eeb5c2245acb45c08",
                SummaryAr = "b1b83e56ec9342b9a9b161d49e27fe1c6101ed4a66134470ae32e6cec3656120d",
                HeaderImage = "f6280b6cb2a24cd8a61e1d54f8e5",
                Image = "1a6dcbdf1cf34294ad2dc138350a8fe1a9c10236b68646db87bf16d1a3fd4c8c",
                Order = 113873686,
                IsActive = true
            };

            // Act
            var serviceResult = await _mediaGalleriesAppService.UpdateAsync(Guid.Parse("22145e05-3258-4612-91df-358782e69b85"), input);

            // Assert
            var result = await _mediaGalleryRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("ed37bb2dd29048db816872d8ddec7");
            result.Slug.ShouldBe("9c290c46a7ab4b6ca87548b177");
            result.TitleAr.ShouldBe("f84b906413d949628b64bb72bc4");
            result.MetaTitleEn.ShouldBe("d357c827f79c440c89883bdd4475d77dff3b");
            result.MetaTitleAr.ShouldBe("b70eae92e84a43bd9");
            result.MetaDescriptionEn.ShouldBe("7873fa343f574906b26111015478c94d3844009a4efa4ed4b121");
            result.MetaDescriptionAr.ShouldBe("a1d22c1e");
            result.SummaryEn.ShouldBe("8d9ec613f4444c039cdc0d2a85ad66769c318960728b40ce92e0187512a2c0f6eeb5c2245acb45c08");
            result.SummaryAr.ShouldBe("b1b83e56ec9342b9a9b161d49e27fe1c6101ed4a66134470ae32e6cec3656120d");
            result.HeaderImage.ShouldBe("f6280b6cb2a24cd8a61e1d54f8e5");
            result.Image.ShouldBe("1a6dcbdf1cf34294ad2dc138350a8fe1a9c10236b68646db87bf16d1a3fd4c8c");
            result.Order.ShouldBe(113873686);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _mediaGalleriesAppService.DeleteAsync(Guid.Parse("22145e05-3258-4612-91df-358782e69b85"));

            // Assert
            var result = await _mediaGalleryRepository.FindAsync(c => c.Id == Guid.Parse("22145e05-3258-4612-91df-358782e69b85"));

            result.ShouldBeNull();
        }
    }
}