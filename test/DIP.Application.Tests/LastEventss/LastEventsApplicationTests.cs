using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.LastEventss
{
    public class LastEventssAppServiceTests : DIPApplicationTestBase
    {
        private readonly ILastEventssAppService _lastEventssAppService;
        private readonly IRepository<LastEvents, Guid> _lastEventsRepository;

        public LastEventssAppServiceTests()
        {
            _lastEventssAppService = GetRequiredService<ILastEventssAppService>();
            _lastEventsRepository = GetRequiredService<IRepository<LastEvents, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _lastEventssAppService.GetListAsync(new GetLastEventssInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("55e1ef36-18f1-4151-9ee2-f7a4b214ad4d")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("e9df8b4a-8c88-4ed6-b9a4-26ba0bd93ce4")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _lastEventssAppService.GetAsync(Guid.Parse("55e1ef36-18f1-4151-9ee2-f7a4b214ad4d"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("55e1ef36-18f1-4151-9ee2-f7a4b214ad4d"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new LastEventsCreateDto
            {
                TitleAr = "15359a0b29614703a13a69f94ecde02df57fcd96eb944af6a3a7b1e4da9dd9d380c",
                TitleEn = "7964b0662a90434c8db26b65f8275fdbde811da36a8e4fa9b2a25fc0cd18",
                MetaTitleAr = "10eab20f9ce44eb0b9d855845a9accadfe91ddc53ca6429f8d8d0d00825057034c58ed4d17c046e9800ff28f4c025c",
                MetaTitleEn = "d0d4be1f51fc44d989c8f9f5586b461b348a2569467a4047a",
                MetaDescriptionEn = "aa3de84894c843d6937da844a2e99a2882d144cfb09a4952ab94c996d22e5045fa315a125c374",
                MetaDescriptionAr = "f00a4e26169940e8bfe4d0beabc4ff5dcd10f66d2e294f9986",
                IsFeatured = true,
                Slug = "5caa47fb10884762",
                Image = "f8daf499963e4b319639ce787531921913e0f94df85049a58c1cb106e215d7691d0d7e01d6d0427d9218",
                HeaderImage = "3a53ab74ed684e22a225698a80db1767cb31ef668887425f",
                DescriptionAr = "6577c81531834a16993e4",
                DescriptionEn = "71baa2df683843fa857dc3a195b984bba3757004c026433b9148c6aef96757b5db54a6",
                SummaryEn = "1b1456126f2c4adc902ceab18c16f570357fd733088c4f3fb1d91c2950b1c90d77e51024ca3c49bb94521",
                SummaryAr = "0904c971de5d465089319bb851b8542e8501c72bb585447c9b6062581fe539e319a8",
                Order = 977360366,
                StartDate = new DateTime(2005, 1, 21),
                EndDate = new DateTime(2004, 10, 19),
                IsActive = true,
                LocationAr = "ff80cef0f9cb4638806fa9c594f63b780bc378885d3345b1a2c0ba9",
                LocationEn = "0370039832b94a0195356c821"
            };

            // Act
            var serviceResult = await _lastEventssAppService.CreateAsync(input);

            // Assert
            var result = await _lastEventsRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleAr.ShouldBe("15359a0b29614703a13a69f94ecde02df57fcd96eb944af6a3a7b1e4da9dd9d380c");
            result.TitleEn.ShouldBe("7964b0662a90434c8db26b65f8275fdbde811da36a8e4fa9b2a25fc0cd18");
            result.MetaTitleAr.ShouldBe("10eab20f9ce44eb0b9d855845a9accadfe91ddc53ca6429f8d8d0d00825057034c58ed4d17c046e9800ff28f4c025c");
            result.MetaTitleEn.ShouldBe("d0d4be1f51fc44d989c8f9f5586b461b348a2569467a4047a");
            result.MetaDescriptionEn.ShouldBe("aa3de84894c843d6937da844a2e99a2882d144cfb09a4952ab94c996d22e5045fa315a125c374");
            result.MetaDescriptionAr.ShouldBe("f00a4e26169940e8bfe4d0beabc4ff5dcd10f66d2e294f9986");
            result.IsFeatured.ShouldBe(true);
            result.Slug.ShouldBe("5caa47fb10884762");
            result.Image.ShouldBe("f8daf499963e4b319639ce787531921913e0f94df85049a58c1cb106e215d7691d0d7e01d6d0427d9218");
            result.HeaderImage.ShouldBe("3a53ab74ed684e22a225698a80db1767cb31ef668887425f");
            result.DescriptionAr.ShouldBe("6577c81531834a16993e4");
            result.DescriptionEn.ShouldBe("71baa2df683843fa857dc3a195b984bba3757004c026433b9148c6aef96757b5db54a6");
            result.SummaryEn.ShouldBe("1b1456126f2c4adc902ceab18c16f570357fd733088c4f3fb1d91c2950b1c90d77e51024ca3c49bb94521");
            result.SummaryAr.ShouldBe("0904c971de5d465089319bb851b8542e8501c72bb585447c9b6062581fe539e319a8");
            result.Order.ShouldBe(977360366);
            result.StartDate.ShouldBe(new DateTime(2005, 1, 21));
            result.EndDate.ShouldBe(new DateTime(2004, 10, 19));
            result.IsActive.ShouldBe(true);
            result.LocationAr.ShouldBe("ff80cef0f9cb4638806fa9c594f63b780bc378885d3345b1a2c0ba9");
            result.LocationEn.ShouldBe("0370039832b94a0195356c821");
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new LastEventsUpdateDto()
            {
                TitleAr = "39199197dde84968a224287e5827f190551432834b02479a82823ca1371f1de02685419db979",
                TitleEn = "d949d91851",
                MetaTitleAr = "4d3f65",
                MetaTitleEn = "8e2f7e6247d840af9ad6c123eedf09a5573c4d87744545bca2f299f422e56504c9db95bcf9024382b8889cf621ea5dc8",
                MetaDescriptionEn = "0d0c292038af4063a932e6272dc8e37725defabba3cb4275a05b0f25f5aa5c57361c241711454390b74fa1dae6e24afb5",
                MetaDescriptionAr = "b311f5a3ebe448458859ab70376ea780e894e52212d64a7",
                IsFeatured = true,
                Slug = "5b15f5e621e847b6b3b434eb2f1b1a70ebf2",
                Image = "b10f26cb144a4ed89dbf1693d276b1212d",
                HeaderImage = "8cda8afb356c4ad6823817c333bb025b4c2b94b288514769a08abbc90dc4dc9347c70c452",
                DescriptionAr = "4ecc6f39375041f4abe60f28bf0b4cea2fcd4464bf6445fcb2d4e49a94b0c8",
                DescriptionEn = "90e579d50a20454abfc3816d05d4a1b9c233c4f8ef3c419aa4455bb9913357930e4cbd75db2d40af",
                SummaryEn = "038a982898594e09a0c7a25a6aaf65b25cff037673464c19b1a748051d857fc4ae0a18d5bc9a4f28a90424500cb20af",
                SummaryAr = "ccccf6651f484b79a2eb397d498ac79387cff24f5ffa41ec886f36929bc0e1368",
                Order = 600432140,
                StartDate = new DateTime(2015, 11, 12),
                EndDate = new DateTime(2019, 10, 25),
                IsActive = true,
                LocationAr = "896fdb2e481147708504aa983f275d789e0ea64554de4bfab2",
                LocationEn = "2d5d6b3b18354a58884e9bdc006c52a1b982ad8ba"
            };

            // Act
            var serviceResult = await _lastEventssAppService.UpdateAsync(Guid.Parse("55e1ef36-18f1-4151-9ee2-f7a4b214ad4d"), input);

            // Assert
            var result = await _lastEventsRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleAr.ShouldBe("39199197dde84968a224287e5827f190551432834b02479a82823ca1371f1de02685419db979");
            result.TitleEn.ShouldBe("d949d91851");
            result.MetaTitleAr.ShouldBe("4d3f65");
            result.MetaTitleEn.ShouldBe("8e2f7e6247d840af9ad6c123eedf09a5573c4d87744545bca2f299f422e56504c9db95bcf9024382b8889cf621ea5dc8");
            result.MetaDescriptionEn.ShouldBe("0d0c292038af4063a932e6272dc8e37725defabba3cb4275a05b0f25f5aa5c57361c241711454390b74fa1dae6e24afb5");
            result.MetaDescriptionAr.ShouldBe("b311f5a3ebe448458859ab70376ea780e894e52212d64a7");
            result.IsFeatured.ShouldBe(true);
            result.Slug.ShouldBe("5b15f5e621e847b6b3b434eb2f1b1a70ebf2");
            result.Image.ShouldBe("b10f26cb144a4ed89dbf1693d276b1212d");
            result.HeaderImage.ShouldBe("8cda8afb356c4ad6823817c333bb025b4c2b94b288514769a08abbc90dc4dc9347c70c452");
            result.DescriptionAr.ShouldBe("4ecc6f39375041f4abe60f28bf0b4cea2fcd4464bf6445fcb2d4e49a94b0c8");
            result.DescriptionEn.ShouldBe("90e579d50a20454abfc3816d05d4a1b9c233c4f8ef3c419aa4455bb9913357930e4cbd75db2d40af");
            result.SummaryEn.ShouldBe("038a982898594e09a0c7a25a6aaf65b25cff037673464c19b1a748051d857fc4ae0a18d5bc9a4f28a90424500cb20af");
            result.SummaryAr.ShouldBe("ccccf6651f484b79a2eb397d498ac79387cff24f5ffa41ec886f36929bc0e1368");
            result.Order.ShouldBe(600432140);
            result.StartDate.ShouldBe(new DateTime(2015, 11, 12));
            result.EndDate.ShouldBe(new DateTime(2019, 10, 25));
            result.IsActive.ShouldBe(true);
            result.LocationAr.ShouldBe("896fdb2e481147708504aa983f275d789e0ea64554de4bfab2");
            result.LocationEn.ShouldBe("2d5d6b3b18354a58884e9bdc006c52a1b982ad8ba");
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _lastEventssAppService.DeleteAsync(Guid.Parse("55e1ef36-18f1-4151-9ee2-f7a4b214ad4d"));

            // Assert
            var result = await _lastEventsRepository.FindAsync(c => c.Id == Guid.Parse("55e1ef36-18f1-4151-9ee2-f7a4b214ad4d"));

            result.ShouldBeNull();
        }
    }
}