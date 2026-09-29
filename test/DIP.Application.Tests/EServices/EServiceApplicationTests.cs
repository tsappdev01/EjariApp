using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.EServices
{
    public class EServicesAppServiceTests : DIPApplicationTestBase
    {
        private readonly IEServicesAppService _eServicesAppService;
        private readonly IRepository<EService, Guid> _eServiceRepository;

        public EServicesAppServiceTests()
        {
            _eServicesAppService = GetRequiredService<IEServicesAppService>();
            _eServiceRepository = GetRequiredService<IRepository<EService, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _eServicesAppService.GetListAsync(new GetEServicesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("7b1f4893-4fa5-419b-9369-9962f6a937aa")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("5ba822b9-0fab-406b-beb8-4eb05cb8e6a0")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _eServicesAppService.GetAsync(Guid.Parse("7b1f4893-4fa5-419b-9369-9962f6a937aa"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("7b1f4893-4fa5-419b-9369-9962f6a937aa"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new EServiceCreateDto
            {
                TitleEn = "ab3c56f0a9134a608df595dde10a266d48db3648c7444761a6259a84dbd4b7e452d2",
                TitleAr = "100380ded81e4b16a345aca0a3dbc5b3f13aeff48e1d4162adb7143bc5487d184d605c5bf61",
                Slug = "7e188c6d1f2d46",
                DescriptionEn = "aa2d9f9c4ba44510882f5437b26a7f76250ea838d12643ffb5c",
                DescriptionAr = "754b2d1fc2bc4f15aca9bbadf5fc6171a251eb57b88c4f1a9314c6a80a08b013038ac1594f3b48c3877e3b6",
                Image = "d1634f31d13a498a974a2f5849cf8ebbedb61d6936374ad89d154477702e9b5d598afc51672c412999",
                HeaderImage = "64e40dd7347c4a5c94e8a0eb4817c4911f60f6619c4941b49fd12709f5e91f07a65",
                MetaTitleEn = "c3f6a48a9e4b49ae8e28d95667e58d146f",
                MetaTitleAr = "b49e65873286459",
                MetaDescriptionEn = "ed48e68f87d24c1ba21ad88b3919515214069747886449f3aafb72a866",
                MetaDescriptionAr = "0ff9c6ee4894404ab029aaf8d4476c243bd14cc9aa2d48ca8dda703e2c7e029568d0cbd01cc0482799683c",
                Order = 648154226,
                IsActive = true
            };

            // Act
            var serviceResult = await _eServicesAppService.CreateAsync(input);

            // Assert
            var result = await _eServiceRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("ab3c56f0a9134a608df595dde10a266d48db3648c7444761a6259a84dbd4b7e452d2");
            result.TitleAr.ShouldBe("100380ded81e4b16a345aca0a3dbc5b3f13aeff48e1d4162adb7143bc5487d184d605c5bf61");
            result.Slug.ShouldBe("7e188c6d1f2d46");
            result.DescriptionEn.ShouldBe("aa2d9f9c4ba44510882f5437b26a7f76250ea838d12643ffb5c");
            result.DescriptionAr.ShouldBe("754b2d1fc2bc4f15aca9bbadf5fc6171a251eb57b88c4f1a9314c6a80a08b013038ac1594f3b48c3877e3b6");
            result.Image.ShouldBe("d1634f31d13a498a974a2f5849cf8ebbedb61d6936374ad89d154477702e9b5d598afc51672c412999");
            result.HeaderImage.ShouldBe("64e40dd7347c4a5c94e8a0eb4817c4911f60f6619c4941b49fd12709f5e91f07a65");
            result.MetaTitleEn.ShouldBe("c3f6a48a9e4b49ae8e28d95667e58d146f");
            result.MetaTitleAr.ShouldBe("b49e65873286459");
            result.MetaDescriptionEn.ShouldBe("ed48e68f87d24c1ba21ad88b3919515214069747886449f3aafb72a866");
            result.MetaDescriptionAr.ShouldBe("0ff9c6ee4894404ab029aaf8d4476c243bd14cc9aa2d48ca8dda703e2c7e029568d0cbd01cc0482799683c");
            result.Order.ShouldBe(648154226);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new EServiceUpdateDto()
            {
                TitleEn = "e7d9a48035344e609b49616f8645358c2d82aaa058d7417c9c957feaf8d5bf007698d5b9bb0",
                TitleAr = "72657c9d1c9d4575a308b9c28634aacfe1776058bc3549ee97e4",
                Slug = "dcbcdcc256e240a5a7c3f184509044fe8c0ad5a7ab1e4763b300600eb0e5b6410f",
                DescriptionEn = "fddedae6d82842978f52d44dbc67696b146c9cbed6c840b89443ed9cde1da6ea34c82386c5ec4d9991060caecd4e2b162",
                DescriptionAr = "7240d2978f50431a92ce878e9787ae05887ea142833b4560b2055ee8431b7658b752540ee2534ba7b260e1d4c0567fd",
                Image = "4ec4d1158519479481557afdaae7c5eb037693cd618b4ea38d2f01bf5f1effc2be430b673",
                HeaderImage = "4524add841c6491dbc281fcdbb178065e5ac1bfc7a2645b891351e888716b078d27e471e5edc465f8748bcba8db84",
                MetaTitleEn = "c385b3ccfe404d8098737270a2ea888006ef58c988774c29a22e5ede457b5c776cb008f111984b949aba015fb",
                MetaTitleAr = "1b57b23dff6947",
                MetaDescriptionEn = "8517d1f3f87547b194944b3aac9d98ccd8ef942bf82b4092b5581db9fb9aecfd8d7917c7a64040f7b8a881c25d91c",
                MetaDescriptionAr = "e9b8d5711ffa474fae10bd799292ae4974df5f331c61",
                Order = 1595056563,
                IsActive = true
            };

            // Act
            var serviceResult = await _eServicesAppService.UpdateAsync(Guid.Parse("7b1f4893-4fa5-419b-9369-9962f6a937aa"), input);

            // Assert
            var result = await _eServiceRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("e7d9a48035344e609b49616f8645358c2d82aaa058d7417c9c957feaf8d5bf007698d5b9bb0");
            result.TitleAr.ShouldBe("72657c9d1c9d4575a308b9c28634aacfe1776058bc3549ee97e4");
            result.Slug.ShouldBe("dcbcdcc256e240a5a7c3f184509044fe8c0ad5a7ab1e4763b300600eb0e5b6410f");
            result.DescriptionEn.ShouldBe("fddedae6d82842978f52d44dbc67696b146c9cbed6c840b89443ed9cde1da6ea34c82386c5ec4d9991060caecd4e2b162");
            result.DescriptionAr.ShouldBe("7240d2978f50431a92ce878e9787ae05887ea142833b4560b2055ee8431b7658b752540ee2534ba7b260e1d4c0567fd");
            result.Image.ShouldBe("4ec4d1158519479481557afdaae7c5eb037693cd618b4ea38d2f01bf5f1effc2be430b673");
            result.HeaderImage.ShouldBe("4524add841c6491dbc281fcdbb178065e5ac1bfc7a2645b891351e888716b078d27e471e5edc465f8748bcba8db84");
            result.MetaTitleEn.ShouldBe("c385b3ccfe404d8098737270a2ea888006ef58c988774c29a22e5ede457b5c776cb008f111984b949aba015fb");
            result.MetaTitleAr.ShouldBe("1b57b23dff6947");
            result.MetaDescriptionEn.ShouldBe("8517d1f3f87547b194944b3aac9d98ccd8ef942bf82b4092b5581db9fb9aecfd8d7917c7a64040f7b8a881c25d91c");
            result.MetaDescriptionAr.ShouldBe("e9b8d5711ffa474fae10bd799292ae4974df5f331c61");
            result.Order.ShouldBe(1595056563);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _eServicesAppService.DeleteAsync(Guid.Parse("7b1f4893-4fa5-419b-9369-9962f6a937aa"));

            // Assert
            var result = await _eServiceRepository.FindAsync(c => c.Id == Guid.Parse("7b1f4893-4fa5-419b-9369-9962f6a937aa"));

            result.ShouldBeNull();
        }
    }
}