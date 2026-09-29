using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using DIP.PressReleases;
using DIP.EntityFrameworkCore;
using Xunit;

namespace DIP.PressReleases
{
    public class PressReleaseRepositoryTests : DIPEntityFrameworkCoreTestBase
    {
        private readonly IPressReleaseRepository _pressReleaseRepository;

        public PressReleaseRepositoryTests()
        {
            _pressReleaseRepository = GetRequiredService<IPressReleaseRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _pressReleaseRepository.GetListAsync(
                    titleAr: "a4aa06e2b9a1494aa27f19c",
                    titleEn: "4c01f62e9bf04e25aadb0ba8c3bc0e0d9fbce25c870542b4acb3a8da0e3e55c",
                    metaTitleAr: "bea85897778641b0891702d073f5dd5627bdd343c0b445d78e291a045492c7f15c568b54a7ec453",
                    metaTitleEn: "6c9663d91ae040c986b47",
                    metaDescriptionEn: "f17db03188474c549a0bf18bf44ed3e9d35328903bdb",
                    metaDescriptionAr: "440fb7f1376145c0abd67e9a09330d3ec",
                    isFeatured: true,
                    slug: "d83dc5ad9c384ff3bc1",
                    image: "9c327cc8e8b645b2a1e276e0440a6533fbd10ad84af348ad877956e2d7faf5c762f1",
                    headerImage: "3fc05afc3a094dcdae966eabb2f80e10018aed38413846d58",
                    descriptionAr: "11f1d80786d04632a9beb339074e46ce3be5d7599b6a453dbf6ac16447060062e323152f31",
                    descriptionEn: "0f6cae604ef4495",
                    summaryEn: "c2a562c4efae447b8a75e7f20c7eb23b0d0eed344f474a54bce307e9f76fee427f85",
                    summaryAr: "659a0365498548fbade9ba60c7765a23fd709d12d1314",
                    isActive: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("692c1e29-d6e7-4ec8-90d5-1b86615e0989"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _pressReleaseRepository.GetCountAsync(
                    titleAr: "7fd742d0f9544126923c4ca66b4fd812ea6cf4f3ec5c4f9a8bec103b3836dc24bc7250409b964",
                    titleEn: "01d708aff49f43fc9b9ab612daf52fdfa",
                    metaTitleAr: "69a43923f5334d5596edc1613aaee62610d4d850e34c43",
                    metaTitleEn: "bd8e693fe08c4553b0e89b247029262f7933ad094fd44f368a35426e274eb6fc5568702db861467b9172",
                    metaDescriptionEn: "72b4d68",
                    metaDescriptionAr: "5e52979a76f444698cd48fa43780e2ad989c546cd56c45f89e245e096103e7ff12d27b14ecbb46c",
                    isFeatured: true,
                    slug: "a5847deb8a6f4b33b4022fc1f8e9e2fcefb61df1974f49a3afe43ad1c2dff81599ed28521bd446238f6",
                    image: "61aa9727a0ca4857817febd6852c70ca62a8eb681a0841048f05613f5f5b41e15380e4cc2ff34ac3ae5c17f3389",
                    headerImage: "987c3fc0",
                    descriptionAr: "f834aeb085a54b088232ddaee6a7cb313b595419f04f45d789082058a3b027b67ad159dd4fa04050a006",
                    descriptionEn: "40007a19456c4774bb11438e6c3a16ed44d4967457914f8393f811a202348d68f0702ae2afa14d9d83262f04fcffa28b971",
                    summaryEn: "a817811311164",
                    summaryAr: "142b8403f32c4feeb4cf123da358825a3707b4116dbc4f818fa371f582286b2e2",
                    isActive: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}