using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.Zones
{
    public class ZonesAppServiceTests : DIPApplicationTestBase
    {
        private readonly IZonesAppService _zonesAppService;
        private readonly IRepository<Zone, Guid> _zoneRepository;

        public ZonesAppServiceTests()
        {
            _zonesAppService = GetRequiredService<IZonesAppService>();
            _zoneRepository = GetRequiredService<IRepository<Zone, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _zonesAppService.GetListAsync(new GetZonesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("cd8acf73-c72b-479b-9c4b-ccc9db024924")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("0216533c-0a82-4107-b1d5-1abcfcedd1db")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _zonesAppService.GetAsync(Guid.Parse("cd8acf73-c72b-479b-9c4b-ccc9db024924"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("cd8acf73-c72b-479b-9c4b-ccc9db024924"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new ZoneCreateDto
            {
                TitleEn = "aab1500227e549f89824a86f04e934bf907b651a5f6441e688772c402c75b390988a194f88e24a7caf9c5ac2",
                TitleAR = "3d7a55",
                MetaTitleEn = "d4c58a831196475b9df4506625c13092edda444c872744b7bd9a8ab50f27d9609eaf9e3a690843998c486ab2d9c807",
                MetaDescriptionEn = "e032d64b8d8c4c609908e3bb379702fa7b8b84bad5094c76a4",
                MetaTitleAr = "f149f9cfe7af40d4adff086f80256eaa9ebf5da6156a40a88edd8c303b5c878d7a3",
                MetaDescriptionAr = "5e8fbeebf1b34d8ca8a71e188f1d23eb86da02dea8b240a4871e536c76f6f8a09f986d92a0884e09a6809af57196ba7",
                Slug = "d03725c68d42411a91c5584dc1ed28502359704d39ef",
                SummaryEn = "853f32227d01492e963bf29ce81083a62bfbbd6b367845a9a86",
                SummaryAr = "2710ec3485854047820c29264943087",
                Image = "6ad9594f53e04968ba6f6081e99599b76d81fd618a8145608be569",
                HeaderImage = "02682ad9a34248ce9ccfae3520df09ab3703422d7640422f93a3339b34b0aad0e1acca0",
                Order = 1871406456,
                IsFeature = true,
                IsActive = true
            };

            // Act
            var serviceResult = await _zonesAppService.CreateAsync(input);

            // Assert
            var result = await _zoneRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("aab1500227e549f89824a86f04e934bf907b651a5f6441e688772c402c75b390988a194f88e24a7caf9c5ac2");
            result.TitleAR.ShouldBe("3d7a55");
            result.MetaTitleEn.ShouldBe("d4c58a831196475b9df4506625c13092edda444c872744b7bd9a8ab50f27d9609eaf9e3a690843998c486ab2d9c807");
            result.MetaDescriptionEn.ShouldBe("e032d64b8d8c4c609908e3bb379702fa7b8b84bad5094c76a4");
            result.MetaTitleAr.ShouldBe("f149f9cfe7af40d4adff086f80256eaa9ebf5da6156a40a88edd8c303b5c878d7a3");
            result.MetaDescriptionAr.ShouldBe("5e8fbeebf1b34d8ca8a71e188f1d23eb86da02dea8b240a4871e536c76f6f8a09f986d92a0884e09a6809af57196ba7");
            result.Slug.ShouldBe("d03725c68d42411a91c5584dc1ed28502359704d39ef");
            result.SummaryEn.ShouldBe("853f32227d01492e963bf29ce81083a62bfbbd6b367845a9a86");
            result.SummaryAr.ShouldBe("2710ec3485854047820c29264943087");
            result.Image.ShouldBe("6ad9594f53e04968ba6f6081e99599b76d81fd618a8145608be569");
            result.HeaderImage.ShouldBe("02682ad9a34248ce9ccfae3520df09ab3703422d7640422f93a3339b34b0aad0e1acca0");
            result.Order.ShouldBe(1871406456);
            result.IsFeature.ShouldBe(true);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new ZoneUpdateDto()
            {
                TitleEn = "1d69fb7dec6a45",
                TitleAR = "ce01a4d322894f7fa21bf31508ca144de44c53",
                MetaTitleEn = "1cb55cb933ef4b1d9c6376d604c953ece5cd455c0d064b20bc2c98893665ac180daf080b120a4a578f0ba47b",
                MetaDescriptionEn = "a446d13a452849409af95675a77c5b37ef000",
                MetaTitleAr = "6a093fce54894f3eae46623f175adc28e177caa6fb324814ad6234a",
                MetaDescriptionAr = "84b67887ff214990a1b8ea3d981302562a801bb65f1844fbb0228bd25a7ba804c",
                Slug = "034e397bdaa948c288857e6c19a3f77d16995181d4244ad2b4fb4dd604f2dc3fed1d6f2583184051841bd1a720860",
                SummaryEn = "713288a30a994eb99cb14c2b4550000de",
                SummaryAr = "b7e1419ed2784360866a73d25c757c0ca0534626eec24005bf38",
                Image = "99efc4f3eb79421693937565a237b81ce97a",
                HeaderImage = "b9a1e8178fd94dd581226c14b7643099cf10e7160c5d44daa2c84753209734c6744d9a134e3b4895a210cc3d9",
                Order = 872598015,
                IsFeature = true,
                IsActive = true
            };

            // Act
            var serviceResult = await _zonesAppService.UpdateAsync(Guid.Parse("cd8acf73-c72b-479b-9c4b-ccc9db024924"), input);

            // Assert
            var result = await _zoneRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("1d69fb7dec6a45");
            result.TitleAR.ShouldBe("ce01a4d322894f7fa21bf31508ca144de44c53");
            result.MetaTitleEn.ShouldBe("1cb55cb933ef4b1d9c6376d604c953ece5cd455c0d064b20bc2c98893665ac180daf080b120a4a578f0ba47b");
            result.MetaDescriptionEn.ShouldBe("a446d13a452849409af95675a77c5b37ef000");
            result.MetaTitleAr.ShouldBe("6a093fce54894f3eae46623f175adc28e177caa6fb324814ad6234a");
            result.MetaDescriptionAr.ShouldBe("84b67887ff214990a1b8ea3d981302562a801bb65f1844fbb0228bd25a7ba804c");
            result.Slug.ShouldBe("034e397bdaa948c288857e6c19a3f77d16995181d4244ad2b4fb4dd604f2dc3fed1d6f2583184051841bd1a720860");
            result.SummaryEn.ShouldBe("713288a30a994eb99cb14c2b4550000de");
            result.SummaryAr.ShouldBe("b7e1419ed2784360866a73d25c757c0ca0534626eec24005bf38");
            result.Image.ShouldBe("99efc4f3eb79421693937565a237b81ce97a");
            result.HeaderImage.ShouldBe("b9a1e8178fd94dd581226c14b7643099cf10e7160c5d44daa2c84753209734c6744d9a134e3b4895a210cc3d9");
            result.Order.ShouldBe(872598015);
            result.IsFeature.ShouldBe(true);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _zonesAppService.DeleteAsync(Guid.Parse("cd8acf73-c72b-479b-9c4b-ccc9db024924"));

            // Assert
            var result = await _zoneRepository.FindAsync(c => c.Id == Guid.Parse("cd8acf73-c72b-479b-9c4b-ccc9db024924"));

            result.ShouldBeNull();
        }
    }
}