using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using DIP.SiteSettings;

namespace DIP.SiteSettings
{
    public class SiteSettingsDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly ISiteSettingRepository _siteSettingRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public SiteSettingsDataSeedContributor(ISiteSettingRepository siteSettingRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _siteSettingRepository = siteSettingRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _siteSettingRepository.InsertAsync(new SiteSetting
            (
                id: Guid.Parse("cb75aa8b-24b3-46bb-9eec-296b00f1c4e8"),
                faceBookLink: "c5328c834a164d5999cdf71530664cb473ab885f22",
                twitterLink: "2e3cf1662387492e9e598d1dc1493497141141a194ef4de5bc",
                instagramLink: "8e1d69303cf14341b5b732514c259aba0f99a06bc5414ad9bf96f42dee2a85326c2021538f0",
                youTubeLink: "c6d1fac7752645d9b85a04fc36022",
                linkedinLink: "f3b70ed3696a4142b148086c03496f0110c9266a1988428ea773052aa2ebcda89d7e6d6f18a943899b024a9badf8111372",
                fireDepartmentPhone: "a0db0a3f6b73491fa2c699ad9e221050992e13c487f945f5b37b3782c2a274b5a2a9a5f469b6422aaacb1b0987",
                emergency1Phone: "d5c37e2a53dc43429aa22ec2426bd42d148c0c08cdcd440dafac9cf7c2bf45b3b59704ec68",
                emergency2Phone: "83472fc2eb3a4",
                policePost: "6349a37ae1f54202b0043f3c36a6a8a5022da54c762441ecbff48a8705a6c54c0ce4a53685154469a9",
                dipEmail: "62bcd9b69f6b45cea1fb06d83a9a58fc2e014227fbaa4f19b5350",
                phone: "9723588a76bf43fabef5207990799cb6fdad2b532d1f4328ba7771",
                pOBox: "b96b6046313540ae804e29d83f72b8ea4d84f057b2844de392e70",
                officeLocationEn: "a6ed4a89744a47a195679c619d67896297b3a8978dcc4759b613f61e4e106c0614e606a369",
                officeLocationAr: "6f1ca6da090d4e5283deb0f95caddf284447da0cc6dd4fa690d",
                siteLink: "435f53f3f5ae4c0b9652139572a9c018f79b2e844a57425aafe9e1ca1322e22259ff248be0f14473b3753c6886",
                pbLocation: "caa54189188a4328b6a9a1e535f7d2705ae567b8231846d38650e4fbfa8025643a90a9168db04d0f97216",
                workDays: "0b028e0f6ff840d2b46aa",
                workHours: "f0dcff44518249b38587d6bc3ca767ec24d500a733814b09b22744036fdb349d",
                ramadanWorkDays: "fac1829a0aed46b5af20bdc46e091b38e8f3b",
                ramadanWorkHours: "448a9aa8c72341f1bdd7cd1bc852ba74481e65e666e14a5b87773cc2823ce5d7bb3d5bf8bb0b4d72",
                fridayWorkHours: "2f737cea032c4a84a06c1e4893ef6c4542d0846a20f041d39060b5e",
                closedDay1: "2f7f3ba4e5054fa68756d1b7fba9b7b9672608eafaa142449bbc4270772cd9aada89852732a64312b6ef76d0a6a26d12b04",
                closedDay2: "18204a63ef984fb2a0334e5ac30ee2617e1151c63e7e4331bc6583aae7db865c1a25fb7506c6"
            ));

            await _siteSettingRepository.InsertAsync(new SiteSetting
            (
                id: Guid.Parse("d866fd10-0596-4b72-9318-618e588c4c2f"),
                faceBookLink: "42f2931320cf4d07828b5a8fe688cef3497ad4fcc40849f2ac6c1baefb404",
                twitterLink: "8065ac7064c848038e6b33a",
                instagramLink: "61ec835b95364de6b348bd",
                youTubeLink: "7eb58acd378a40c8930c21125e633512d9ec0aea5f674839a845f08454890d5f4c8f520d064643d388679d20cc3f5f0b",
                linkedinLink: "453374d6592d4f8b968d93074fc50865a8a79e18d8aa47bbaf",
                fireDepartmentPhone: "341476f721404bf0a52d5efb9e3c95c790fc9b23c7e646b0972a6f0cf09f254341",
                emergency1Phone: "f187afedaa824c9c807a336d508ffa96a76f2157ad32486585a7658",
                emergency2Phone: "7308c68f15634c1ca3bdad172e692d8c63e0c4acb8a54701b58e005e5dc2fa9c19920bd0adec462ab6d2ff123fa",
                policePost: "dffb1c12e1b849969f4ec0551f9",
                dipEmail: "9cce56c6438f470588f1935a54882a3888d37b9bbaee46b7b6f31a60ac1ba99cbe2a142411454",
                phone: "132b366492964065b834988ec8b97e80a3ac3fe4314d419ca7560d95e807e",
                pOBox: "ab4173d17fc44e78b633dfec12ec60a9545d2",
                officeLocationEn: "5f1fddfe9fdb48fea4fcf9f82cdf2abbdea7c777378b4c2e972f598aa37b",
                officeLocationAr: "c1493fafbfbc4db495f8395855f8f9931c4d8dd94",
                siteLink: "87923c6a96fa474d93c638dfe1",
                pbLocation: "bd3158a707d94aab98587503bcc5d14b0cd78fa45d274f61b1de6fb7f93ec19b5a516eeb05c",
                workDays: "3ed5431d3e7045ef8e148b6f547ea339d1e455b6f8e84756ba48ae",
                workHours: "ec025cb7fb8345a782b8155",
                ramadanWorkDays: "43a3212c082b48fc9d7bd9e1e2e4cb6ef97a8d5f7d424ace8432f2dfe8178f091a648a785fb34090b754",
                ramadanWorkHours: "d7c9a4adc76948e48bda2ab1961b557a78b48353e4bf4c7ba4fc3140d189f9962ef6f8b9b35d409c9b8c6",
                fridayWorkHours: "f198c5d59015408ab584d64ad04ba94d1b517c224037406f93395804d3b7b",
                closedDay1: "1a66285723a6406da136113f975c911c8af2b37ac9d94c2390b83cbc381a48c2bb8f33152d3c4ed48",
                closedDay2: "6b13fc7f532c4"
            ));

            await _unitOfWorkManager.Current.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}