using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.PageInfos
{
    public class PageInfosAppServiceTests : DIPApplicationTestBase
    {
        private readonly IPageInfosAppService _pageInfosAppService;
        private readonly IRepository<PageInfo, Guid> _pageInfoRepository;

        public PageInfosAppServiceTests()
        {
            _pageInfosAppService = GetRequiredService<IPageInfosAppService>();
            _pageInfoRepository = GetRequiredService<IRepository<PageInfo, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _pageInfosAppService.GetListAsync(new GetPageInfosInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("83f08243-5154-47a4-a489-639a5bb921f4")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("6298955e-d7d8-4088-8b8c-de1a3aacd46a")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _pageInfosAppService.GetAsync(Guid.Parse("83f08243-5154-47a4-a489-639a5bb921f4"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("83f08243-5154-47a4-a489-639a5bb921f4"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new PageInfoCreateDto
            {
                TitleAr = "9b0e0655f0ed49ddbd8e77dbb03fade",
                TitleEn = "6002a23536954205a581812182e2451814e4f724e769407ab506bd1e2c0ba681c28e86f1a0574522a434251262acf4",
                MetaTitleAr = "9ce4614370f14dce81113",
                MetaTitleEn = "f1887d15e60b440ab04f2d7d12e7495c7afa2e3956ce43959ac89df3919c2a22256309eb9abe40",
                MetaDescriptionEn = "6c13c064f65348eb870d8c13cc89c5575809e85c058e4dde8a746f63f0e89ff3f67cff152f",
                MetaDescriptionAr = "d84d4061d89f47ccbf173",
                Slug = "17f5e08e441e4c66b6ed5678abc91bf22e15fab3fe3f4c9b8858b3905a5ac945387a1114c14f4f17b7d1",
                Image = "d72e0aee1e074e54b828365bdb12c38c91662651c8274d4d87a3e98",
                HeaderImage = "a0d45c58a5a545af88",
                YouTubeUrl = "86b34625e94c46cc8d14f67e827fd01c8013ae41646b4ce8932fe1fb6302e3ac",
                PageInfoArticleTilteEn = "99899d65ed604ccf8aef",
                PageInfoArticleTilteAr = "e2d3a7c3c18a4071bb22524c8e8905d53250ca091b5348449ce1d8e3d513ff6def3099bec41c48",
                PageInfoArticleSubtitleEn = "c022b55b6a304b56ba9ce6257f8019a593b2db647e9a45e694e5642a2b0028fdd962cf12f90e402a813467e3",
                PageInfoArticleSubtitleAr = "18591507d95841709d749e186371e54de95d4ecdad9d415ca274042300ca84f8183934435f404f02a1e3a5c99b6f7a10",
                DescriptionAr = "5333ed3ba8f94c4b8069447f64a2fc707ca192b231284d3",
                DescriptionEn = "445035284f1d4271a39b2952291fb547da82fa0d2b8e47c59dea3c6d808c38bd69ece5318c3b44359eab7349eddf1b30",
                SummaryEn = "5b5e5909d28749cd97a6b8b00",
                SummaryAr = "a233bc6262f14a77b0defb465d876123e54c9e3990e04d858",
                Order = 182285482,
                IsActive = true
            };

            // Act
            var serviceResult = await _pageInfosAppService.CreateAsync(input);

            // Assert
            var result = await _pageInfoRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleAr.ShouldBe("9b0e0655f0ed49ddbd8e77dbb03fade");
            result.TitleEn.ShouldBe("6002a23536954205a581812182e2451814e4f724e769407ab506bd1e2c0ba681c28e86f1a0574522a434251262acf4");
            result.MetaTitleAr.ShouldBe("9ce4614370f14dce81113");
            result.MetaTitleEn.ShouldBe("f1887d15e60b440ab04f2d7d12e7495c7afa2e3956ce43959ac89df3919c2a22256309eb9abe40");
            result.MetaDescriptionEn.ShouldBe("6c13c064f65348eb870d8c13cc89c5575809e85c058e4dde8a746f63f0e89ff3f67cff152f");
            result.MetaDescriptionAr.ShouldBe("d84d4061d89f47ccbf173");
            result.Slug.ShouldBe("17f5e08e441e4c66b6ed5678abc91bf22e15fab3fe3f4c9b8858b3905a5ac945387a1114c14f4f17b7d1");
            result.Image.ShouldBe("d72e0aee1e074e54b828365bdb12c38c91662651c8274d4d87a3e98");
            result.HeaderImage.ShouldBe("a0d45c58a5a545af88");
            result.YouTubeUrl.ShouldBe("86b34625e94c46cc8d14f67e827fd01c8013ae41646b4ce8932fe1fb6302e3ac");
            result.PageInfoArticleTilteEn.ShouldBe("99899d65ed604ccf8aef");
            result.PageInfoArticleTilteAr.ShouldBe("e2d3a7c3c18a4071bb22524c8e8905d53250ca091b5348449ce1d8e3d513ff6def3099bec41c48");
            result.PageInfoArticleSubtitleEn.ShouldBe("c022b55b6a304b56ba9ce6257f8019a593b2db647e9a45e694e5642a2b0028fdd962cf12f90e402a813467e3");
            result.PageInfoArticleSubtitleAr.ShouldBe("18591507d95841709d749e186371e54de95d4ecdad9d415ca274042300ca84f8183934435f404f02a1e3a5c99b6f7a10");
            result.DescriptionAr.ShouldBe("5333ed3ba8f94c4b8069447f64a2fc707ca192b231284d3");
            result.DescriptionEn.ShouldBe("445035284f1d4271a39b2952291fb547da82fa0d2b8e47c59dea3c6d808c38bd69ece5318c3b44359eab7349eddf1b30");
            result.SummaryEn.ShouldBe("5b5e5909d28749cd97a6b8b00");
            result.SummaryAr.ShouldBe("a233bc6262f14a77b0defb465d876123e54c9e3990e04d858");
            result.Order.ShouldBe(182285482);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new PageInfoUpdateDto()
            {
                TitleAr = "deb32f10965e4caf822a97cfd279183",
                TitleEn = "7e6d3b6aa78e457f9e7db81a4d910b41b94",
                MetaTitleAr = "0169732582344e89bf498ec9115bed5",
                MetaTitleEn = "d3f400285b944050973",
                MetaDescriptionEn = "cbca909814ab480e89cca",
                MetaDescriptionAr = "c6c865117637433ba8231617e3cf25faf37d5631",
                Slug = "4f5f978aacdf4861a7c23173954c19ab8630b5b6f9d0437e9f1df8c738bcd53e07f048d6337249d0852abd8",
                Image = "e661a4f8335e4efd9bf82927011924b4d1f561cbe0d842828a16a55869cc7af",
                HeaderImage = "1369b13a2a0346778d",
                YouTubeUrl = "dbb4300046c44013a4d2aae4ff2d3cd6bff2f999b31544acae5",
                PageInfoArticleTilteEn = "8543d438307a42efb0b0ff8859f632294eb4ca2bd",
                PageInfoArticleTilteAr = "4c2420da226e4d36803a6688d28cb9c964d3a62d6c2a4a4d810b947b6",
                PageInfoArticleSubtitleEn = "3297b18adc30479faa3ac77a",
                PageInfoArticleSubtitleAr = "017ac5a9d6194ccea3e9479427cd9878e28ee1d9957d40fbbc27fb43250f869d3c1eb491600840e4b359e2c8c",
                DescriptionAr = "e737c0216c05474e8f40dd9639c02228f4b9cba7371b4769be456101c645d22d5485e1bb315a4a77838df66",
                DescriptionEn = "1c950ea88798493ab74b71bf97c6d184bc8218fe99fb49acb1895b89ce4a79bc0398fd87cb9445c98dc351c394e21a6ffb",
                SummaryEn = "57de9d3165a64124b165527026059d1f7368504b341741138e793c2bf30d",
                SummaryAr = "11913e287935472eafa11693132896ce5",
                Order = 53906512,
                IsActive = true
            };

            // Act
            var serviceResult = await _pageInfosAppService.UpdateAsync(Guid.Parse("83f08243-5154-47a4-a489-639a5bb921f4"), input);

            // Assert
            var result = await _pageInfoRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleAr.ShouldBe("deb32f10965e4caf822a97cfd279183");
            result.TitleEn.ShouldBe("7e6d3b6aa78e457f9e7db81a4d910b41b94");
            result.MetaTitleAr.ShouldBe("0169732582344e89bf498ec9115bed5");
            result.MetaTitleEn.ShouldBe("d3f400285b944050973");
            result.MetaDescriptionEn.ShouldBe("cbca909814ab480e89cca");
            result.MetaDescriptionAr.ShouldBe("c6c865117637433ba8231617e3cf25faf37d5631");
            result.Slug.ShouldBe("4f5f978aacdf4861a7c23173954c19ab8630b5b6f9d0437e9f1df8c738bcd53e07f048d6337249d0852abd8");
            result.Image.ShouldBe("e661a4f8335e4efd9bf82927011924b4d1f561cbe0d842828a16a55869cc7af");
            result.HeaderImage.ShouldBe("1369b13a2a0346778d");
            result.YouTubeUrl.ShouldBe("dbb4300046c44013a4d2aae4ff2d3cd6bff2f999b31544acae5");
            result.PageInfoArticleTilteEn.ShouldBe("8543d438307a42efb0b0ff8859f632294eb4ca2bd");
            result.PageInfoArticleTilteAr.ShouldBe("4c2420da226e4d36803a6688d28cb9c964d3a62d6c2a4a4d810b947b6");
            result.PageInfoArticleSubtitleEn.ShouldBe("3297b18adc30479faa3ac77a");
            result.PageInfoArticleSubtitleAr.ShouldBe("017ac5a9d6194ccea3e9479427cd9878e28ee1d9957d40fbbc27fb43250f869d3c1eb491600840e4b359e2c8c");
            result.DescriptionAr.ShouldBe("e737c0216c05474e8f40dd9639c02228f4b9cba7371b4769be456101c645d22d5485e1bb315a4a77838df66");
            result.DescriptionEn.ShouldBe("1c950ea88798493ab74b71bf97c6d184bc8218fe99fb49acb1895b89ce4a79bc0398fd87cb9445c98dc351c394e21a6ffb");
            result.SummaryEn.ShouldBe("57de9d3165a64124b165527026059d1f7368504b341741138e793c2bf30d");
            result.SummaryAr.ShouldBe("11913e287935472eafa11693132896ce5");
            result.Order.ShouldBe(53906512);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _pageInfosAppService.DeleteAsync(Guid.Parse("83f08243-5154-47a4-a489-639a5bb921f4"));

            // Assert
            var result = await _pageInfoRepository.FindAsync(c => c.Id == Guid.Parse("83f08243-5154-47a4-a489-639a5bb921f4"));

            result.ShouldBeNull();
        }
    }
}