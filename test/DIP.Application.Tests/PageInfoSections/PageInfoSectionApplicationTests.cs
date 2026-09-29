using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.PageInfoSections
{
    public class PageInfoSectionsAppServiceTests : DIPApplicationTestBase
    {
        private readonly IPageInfoSectionsAppService _pageInfoSectionsAppService;
        private readonly IRepository<PageInfoSection, Guid> _pageInfoSectionRepository;

        public PageInfoSectionsAppServiceTests()
        {
            _pageInfoSectionsAppService = GetRequiredService<IPageInfoSectionsAppService>();
            _pageInfoSectionRepository = GetRequiredService<IRepository<PageInfoSection, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _pageInfoSectionsAppService.GetListAsync(new GetPageInfoSectionsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.PageInfoSection.Id == Guid.Parse("edc397e5-c605-4cb2-bd10-6ab986f655f6")).ShouldBe(true);
            result.Items.Any(x => x.PageInfoSection.Id == Guid.Parse("838e80ff-792a-44e9-a2d7-4cabe6105c3a")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _pageInfoSectionsAppService.GetAsync(Guid.Parse("edc397e5-c605-4cb2-bd10-6ab986f655f6"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("edc397e5-c605-4cb2-bd10-6ab986f655f6"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new PageInfoSectionCreateDto
            {
                TitleEn = "dd9078b055e54189b77e7ae7baa22c58e25d50af62",
                TitleAr = "f00b6bd98d2746cb8251023062b3d3ead7c704b408d745579f25ad177414b45bb5eda315391d46c2bdca1",
                SubTitleEn = "5bd4a544d170421c91f6625012f7",
                SubTitleAr = "42ecb1ee",
                SummaryEn = "5cf0df21e17a41c8a873820e35d2c32430b16114b35949e6b",
                SummaryAr = "31fef47a083e45f988c49e4aac768f13085",
                DescriptionEn = "ef6b3b3c",
                DescriptionAr = "cb90fe7add98437b82c2f80f7c98512599cc62d6644",
                PageSectionMedia = "cba588dc76fe403b9349082c072d85c1668935ebe56f4d0297e91b22249e9d241fec9bf330f54115ad9048674f",
                YoutubeUrl = "ba222a260caa46f88c05357077a77decdc3ac0e9d4a8471faf32fb3918a207a2b60aa31442664c73a0a51e6a46",
                Order = 40753377,
                IsActive = true,
                PageInfoId = Guid.Parse("c9631135-a498-4113-b1b2-36e40f9baa78")
            };

            // Act
            var serviceResult = await _pageInfoSectionsAppService.CreateAsync(input);

            // Assert
            var result = await _pageInfoSectionRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("dd9078b055e54189b77e7ae7baa22c58e25d50af62");
            result.TitleAr.ShouldBe("f00b6bd98d2746cb8251023062b3d3ead7c704b408d745579f25ad177414b45bb5eda315391d46c2bdca1");
            result.SubTitleEn.ShouldBe("5bd4a544d170421c91f6625012f7");
            result.SubTitleAr.ShouldBe("42ecb1ee");
            result.SummaryEn.ShouldBe("5cf0df21e17a41c8a873820e35d2c32430b16114b35949e6b");
            result.SummaryAr.ShouldBe("31fef47a083e45f988c49e4aac768f13085");
            result.DescriptionEn.ShouldBe("ef6b3b3c");
            result.DescriptionAr.ShouldBe("cb90fe7add98437b82c2f80f7c98512599cc62d6644");
            result.PageSectionMedia.ShouldBe("cba588dc76fe403b9349082c072d85c1668935ebe56f4d0297e91b22249e9d241fec9bf330f54115ad9048674f");
            result.YoutubeUrl.ShouldBe("ba222a260caa46f88c05357077a77decdc3ac0e9d4a8471faf32fb3918a207a2b60aa31442664c73a0a51e6a46");
            result.Order.ShouldBe(40753377);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new PageInfoSectionUpdateDto()
            {
                TitleEn = "5d8c296436234b1aa96ef86704f0103781d0a03fc94340b78da792abd",
                TitleAr = "dac045e1df3b4c72a28b1320196206ee1138b92b33d846cf89d29e",
                SubTitleEn = "3350bd3d4e6c4a50b67deb4c128ae7dba30a35af73114f0a9a6",
                SubTitleAr = "cfd4561e3bf4417080835d43088cd597e4266bf1d18b4026a45482f2313ead8c47d2c85215a04e368c3",
                SummaryEn = "db87e8d3a85147ee89533a9c22ed35",
                SummaryAr = "83eeb9eec111",
                DescriptionEn = "a51bd8a7e7a54ec59618b877fb9730eb516e52fff4c249909b2ec7c96492756eda93282c65f74fa88b7aab1f1",
                DescriptionAr = "0a676ff7b780429",
                PageSectionMedia = "62ffbe15280e4f3ca380464d76b8e17c582e9a1882b24944a831de46cedad8d15a741031",
                YoutubeUrl = "0a434ba0655c4d57a525a117df2c1e293f4856c9611e402294fc9e9e7cb4089bc1e8ffdc0",
                Order = 1496277103,
                IsActive = true,
                PageInfoId = Guid.Parse("c9631135-a498-4113-b1b2-36e40f9baa78")
            };

            // Act
            var serviceResult = await _pageInfoSectionsAppService.UpdateAsync(Guid.Parse("edc397e5-c605-4cb2-bd10-6ab986f655f6"), input);

            // Assert
            var result = await _pageInfoSectionRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("5d8c296436234b1aa96ef86704f0103781d0a03fc94340b78da792abd");
            result.TitleAr.ShouldBe("dac045e1df3b4c72a28b1320196206ee1138b92b33d846cf89d29e");
            result.SubTitleEn.ShouldBe("3350bd3d4e6c4a50b67deb4c128ae7dba30a35af73114f0a9a6");
            result.SubTitleAr.ShouldBe("cfd4561e3bf4417080835d43088cd597e4266bf1d18b4026a45482f2313ead8c47d2c85215a04e368c3");
            result.SummaryEn.ShouldBe("db87e8d3a85147ee89533a9c22ed35");
            result.SummaryAr.ShouldBe("83eeb9eec111");
            result.DescriptionEn.ShouldBe("a51bd8a7e7a54ec59618b877fb9730eb516e52fff4c249909b2ec7c96492756eda93282c65f74fa88b7aab1f1");
            result.DescriptionAr.ShouldBe("0a676ff7b780429");
            result.PageSectionMedia.ShouldBe("62ffbe15280e4f3ca380464d76b8e17c582e9a1882b24944a831de46cedad8d15a741031");
            result.YoutubeUrl.ShouldBe("0a434ba0655c4d57a525a117df2c1e293f4856c9611e402294fc9e9e7cb4089bc1e8ffdc0");
            result.Order.ShouldBe(1496277103);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _pageInfoSectionsAppService.DeleteAsync(Guid.Parse("edc397e5-c605-4cb2-bd10-6ab986f655f6"));

            // Assert
            var result = await _pageInfoSectionRepository.FindAsync(c => c.Id == Guid.Parse("edc397e5-c605-4cb2-bd10-6ab986f655f6"));

            result.ShouldBeNull();
        }
    }
}