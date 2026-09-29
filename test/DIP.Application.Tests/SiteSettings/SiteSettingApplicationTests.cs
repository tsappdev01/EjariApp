using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.SiteSettings
{
    public class SiteSettingsAppServiceTests : DIPApplicationTestBase
    {
        private readonly ISiteSettingsAppService _siteSettingsAppService;
        private readonly IRepository<SiteSetting, Guid> _siteSettingRepository;

        public SiteSettingsAppServiceTests()
        {
            _siteSettingsAppService = GetRequiredService<ISiteSettingsAppService>();
            _siteSettingRepository = GetRequiredService<IRepository<SiteSetting, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _siteSettingsAppService.GetListAsync(new GetSiteSettingsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("cb75aa8b-24b3-46bb-9eec-296b00f1c4e8")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("d866fd10-0596-4b72-9318-618e588c4c2f")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _siteSettingsAppService.GetAsync(Guid.Parse("cb75aa8b-24b3-46bb-9eec-296b00f1c4e8"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("cb75aa8b-24b3-46bb-9eec-296b00f1c4e8"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new SiteSettingCreateDto
            {
                FaceBookLink = "a20c4a4de4ee418eb856e6a52c67",
                TwitterLink = "2d14d4bb6fa04c8e85e0754e4a7849ed7554a52b0",
                InstagramLink = "a3a8286f89cf46a2a4084c31159a40d793fb2395c0da4a23b2",
                YouTubeLink = "bbc23d25c45c42dda4b313a",
                LinkedinLink = "87546d2f03fa4c0fa6c475c853fb",
                FireDepartmentPhone = "d3824d378",
                Emergency1Phone = "48dfca63a31249e29cb4ec251125c6dbe1a5697e615c4ba59b994bbf5b986f6c8d2ceea0de6a4cffa456355ca88783979",
                Emergency2Phone = "abc2fdf40f554c1d920efe31167794e7fec020cb988843cb842eed07c1dfb6c3a267362ade8b",
                PolicePost = "017c9a095fd742d2ad5aa",
                DipEmail = "c2cc448718d1440d84f8ccc94559a38c4f6e39e8dff84efb873c82af9fd38f55cf4bd8d429f74d08a11f8",
                Phone = "69de6d2f27da41d896872b5d90c22ae97eef3b679fb9423b9e85b19d10dda929",
                POBox = "133d5c1a279242438defe4b53cbf4084c16d31e7c",
                OfficeLocationEn = "6306b53703bd45b4a202c15",
                OfficeLocationAr = "770f74802cbb4e5ba712913d5de2d0030a5127b00e7549ce",
                SiteLink = "6b8deb55a6b84c6d96e12b207cb9d6671571dee1e01b49ca8b78721f473f8dad51b838b49dd8450fb",
                PbLocation = "455467fad6b6467abab35484fd4",
                WorkDays = "8ba5b2ef567842ee8b9f5f2834e957",
                WorkHours = "b8dff2268",
                RamadanWorkDays = "793db08d2fb44e",
                RamadanWorkHours = "853e19b0037d476ca87560efeaf8960180769f70678a4cfe80ef2e",
                FridayWorkHours = "03dba83612fc4f8d872a2ec8275dba426c162929fe114706b90c30ff7588c466fe26688f05034",
                ClosedDay1 = "5980d8199bf84027be1fd7b91473a7a65a03259ad84b401eb686fccfe4221",
                ClosedDay2 = "36b7e7861c"
            };

            // Act
            var serviceResult = await _siteSettingsAppService.CreateAsync(input);

            // Assert
            var result = await _siteSettingRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.FaceBookLink.ShouldBe("a20c4a4de4ee418eb856e6a52c67");
            result.TwitterLink.ShouldBe("2d14d4bb6fa04c8e85e0754e4a7849ed7554a52b0");
            result.InstagramLink.ShouldBe("a3a8286f89cf46a2a4084c31159a40d793fb2395c0da4a23b2");
            result.YouTubeLink.ShouldBe("bbc23d25c45c42dda4b313a");
            result.LinkedinLink.ShouldBe("87546d2f03fa4c0fa6c475c853fb");
            result.FireDepartmentPhone.ShouldBe("d3824d378");
            result.Emergency1Phone.ShouldBe("48dfca63a31249e29cb4ec251125c6dbe1a5697e615c4ba59b994bbf5b986f6c8d2ceea0de6a4cffa456355ca88783979");
            result.Emergency2Phone.ShouldBe("abc2fdf40f554c1d920efe31167794e7fec020cb988843cb842eed07c1dfb6c3a267362ade8b");
            result.PolicePost.ShouldBe("017c9a095fd742d2ad5aa");
            result.DipEmail.ShouldBe("c2cc448718d1440d84f8ccc94559a38c4f6e39e8dff84efb873c82af9fd38f55cf4bd8d429f74d08a11f8");
            result.Phone.ShouldBe("69de6d2f27da41d896872b5d90c22ae97eef3b679fb9423b9e85b19d10dda929");
            result.POBox.ShouldBe("133d5c1a279242438defe4b53cbf4084c16d31e7c");
            result.OfficeLocationEn.ShouldBe("6306b53703bd45b4a202c15");
            result.OfficeLocationAr.ShouldBe("770f74802cbb4e5ba712913d5de2d0030a5127b00e7549ce");
            result.SiteLink.ShouldBe("6b8deb55a6b84c6d96e12b207cb9d6671571dee1e01b49ca8b78721f473f8dad51b838b49dd8450fb");
            result.PbLocation.ShouldBe("455467fad6b6467abab35484fd4");
            result.WorkDays.ShouldBe("8ba5b2ef567842ee8b9f5f2834e957");
            result.WorkHours.ShouldBe("b8dff2268");
            result.RamadanWorkDays.ShouldBe("793db08d2fb44e");
            result.RamadanWorkHours.ShouldBe("853e19b0037d476ca87560efeaf8960180769f70678a4cfe80ef2e");
            result.FridayWorkHours.ShouldBe("03dba83612fc4f8d872a2ec8275dba426c162929fe114706b90c30ff7588c466fe26688f05034");
            result.ClosedDay1.ShouldBe("5980d8199bf84027be1fd7b91473a7a65a03259ad84b401eb686fccfe4221");
            result.ClosedDay2.ShouldBe("36b7e7861c");
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new SiteSettingUpdateDto()
            {
                FaceBookLink = "2c6f658d34294e4b8628cc36158416dca428b4e368bb437eb0c6f4e71164d62ab6ca",
                TwitterLink = "2047eb076d454188aae5a6fb2c79c14ee1367f874ed24136baaba03929ae0082",
                InstagramLink = "bd89fb9abc4f4eb0a42d0b7e25537473e8edf0e24a934a5bb98b7401c55e1ef62af2b29a",
                YouTubeLink = "035b4f14541945",
                LinkedinLink = "433a5a0ebe794b85a130a5fc1134",
                FireDepartmentPhone = "6738647e3a854926bcd6d9854bfd1ec659e6cff541c5427293134a956dbdee3c47f19735069c42c793b440e5dba",
                Emergency1Phone = "7032c2043c6148c9a197",
                Emergency2Phone = "554b71ce76d94ddea36c98387530e532e479e999064b4231bb03871af05ca6cd295e05",
                PolicePost = "30489a036a594e038a2806ff4789f7ba5956fc66c24648379251",
                DipEmail = "f698eb9e6c7545109e4043d8922be87909d999b713e1433d975c198a9e55aa3d0688f742d23f46ca8304d31fcf",
                Phone = "a050c6b7a1ad4f0b9",
                POBox = "099108e9b4f241b08c945c216b5148dbaf1df7a1352c47788e5d2b6e537477ed88bb8138352",
                OfficeLocationEn = "df43fd63c56f4ad28735267e211024d85cf7519a02cd483f89b6d14ad39fceb48f38fe982d464f48adabbf9",
                OfficeLocationAr = "d881a4292",
                SiteLink = "110c109b29a14510a2d957762b069ed007d41cbc35bc494dadd63fae019736579e5b102f0e454f8c9f14028b0fbfd614c9",
                PbLocation = "3edfd45cf2d3416e857571f254b53578c7ae39b7071e42cabd486fcdc3d149b14f2b3007e8dd405f945025c46da2d52556d",
                WorkDays = "5ffc43add76a47f88c83521692d5ab23a5affb0b05464c7b9415a9b7984a6f6e9c7a814ebd7b",
                WorkHours = "c0f6e83d64f54603ac9c63f4aaa158216353ad25baf84b28b2000ff8378b4b125fc8094c744245ac94201240e",
                RamadanWorkDays = "57dc43515bd",
                RamadanWorkHours = "5ad93fc9f9b7424f809c5d2349c533c9655d71139d0",
                FridayWorkHours = "3eceb9bad94644c4a1cd119e02c35e5272d1bcb27c7548329991c7301b13015961169c28d",
                ClosedDay1 = "540d026bfca84a9888e3161c2fad62cff8787724a6254611a106696c21f2398008e1a16b2fb343c38e2891a",
                ClosedDay2 = "6dd440aa148d42b1b8bda711f50c01c41b9239c34a2c48b3a90058f9f40b3c43c5a4d83a370"
            };

            // Act
            var serviceResult = await _siteSettingsAppService.UpdateAsync(Guid.Parse("cb75aa8b-24b3-46bb-9eec-296b00f1c4e8"), input);

            // Assert
            var result = await _siteSettingRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.FaceBookLink.ShouldBe("2c6f658d34294e4b8628cc36158416dca428b4e368bb437eb0c6f4e71164d62ab6ca");
            result.TwitterLink.ShouldBe("2047eb076d454188aae5a6fb2c79c14ee1367f874ed24136baaba03929ae0082");
            result.InstagramLink.ShouldBe("bd89fb9abc4f4eb0a42d0b7e25537473e8edf0e24a934a5bb98b7401c55e1ef62af2b29a");
            result.YouTubeLink.ShouldBe("035b4f14541945");
            result.LinkedinLink.ShouldBe("433a5a0ebe794b85a130a5fc1134");
            result.FireDepartmentPhone.ShouldBe("6738647e3a854926bcd6d9854bfd1ec659e6cff541c5427293134a956dbdee3c47f19735069c42c793b440e5dba");
            result.Emergency1Phone.ShouldBe("7032c2043c6148c9a197");
            result.Emergency2Phone.ShouldBe("554b71ce76d94ddea36c98387530e532e479e999064b4231bb03871af05ca6cd295e05");
            result.PolicePost.ShouldBe("30489a036a594e038a2806ff4789f7ba5956fc66c24648379251");
            result.DipEmail.ShouldBe("f698eb9e6c7545109e4043d8922be87909d999b713e1433d975c198a9e55aa3d0688f742d23f46ca8304d31fcf");
            result.Phone.ShouldBe("a050c6b7a1ad4f0b9");
            result.POBox.ShouldBe("099108e9b4f241b08c945c216b5148dbaf1df7a1352c47788e5d2b6e537477ed88bb8138352");
            result.OfficeLocationEn.ShouldBe("df43fd63c56f4ad28735267e211024d85cf7519a02cd483f89b6d14ad39fceb48f38fe982d464f48adabbf9");
            result.OfficeLocationAr.ShouldBe("d881a4292");
            result.SiteLink.ShouldBe("110c109b29a14510a2d957762b069ed007d41cbc35bc494dadd63fae019736579e5b102f0e454f8c9f14028b0fbfd614c9");
            result.PbLocation.ShouldBe("3edfd45cf2d3416e857571f254b53578c7ae39b7071e42cabd486fcdc3d149b14f2b3007e8dd405f945025c46da2d52556d");
            result.WorkDays.ShouldBe("5ffc43add76a47f88c83521692d5ab23a5affb0b05464c7b9415a9b7984a6f6e9c7a814ebd7b");
            result.WorkHours.ShouldBe("c0f6e83d64f54603ac9c63f4aaa158216353ad25baf84b28b2000ff8378b4b125fc8094c744245ac94201240e");
            result.RamadanWorkDays.ShouldBe("57dc43515bd");
            result.RamadanWorkHours.ShouldBe("5ad93fc9f9b7424f809c5d2349c533c9655d71139d0");
            result.FridayWorkHours.ShouldBe("3eceb9bad94644c4a1cd119e02c35e5272d1bcb27c7548329991c7301b13015961169c28d");
            result.ClosedDay1.ShouldBe("540d026bfca84a9888e3161c2fad62cff8787724a6254611a106696c21f2398008e1a16b2fb343c38e2891a");
            result.ClosedDay2.ShouldBe("6dd440aa148d42b1b8bda711f50c01c41b9239c34a2c48b3a90058f9f40b3c43c5a4d83a370");
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _siteSettingsAppService.DeleteAsync(Guid.Parse("cb75aa8b-24b3-46bb-9eec-296b00f1c4e8"));

            // Assert
            var result = await _siteSettingRepository.FindAsync(c => c.Id == Guid.Parse("cb75aa8b-24b3-46bb-9eec-296b00f1c4e8"));

            result.ShouldBeNull();
        }
    }
}