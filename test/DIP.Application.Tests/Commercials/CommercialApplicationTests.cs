using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.Commercials
{
    public class CommercialsAppServiceTests : DIPApplicationTestBase
    {
        private readonly ICommercialsAppService _commercialsAppService;
        private readonly IRepository<Commercial, Guid> _commercialRepository;

        public CommercialsAppServiceTests()
        {
            _commercialsAppService = GetRequiredService<ICommercialsAppService>();
            _commercialRepository = GetRequiredService<IRepository<Commercial, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _commercialsAppService.GetListAsync(new GetCommercialsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Commercial.Id == Guid.Parse("643289cf-49ba-4b11-8aa8-e229cafdfca8")).ShouldBe(true);
            result.Items.Any(x => x.Commercial.Id == Guid.Parse("458d807e-5ae7-447b-9877-0891f4becab0")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _commercialsAppService.GetAsync(Guid.Parse("643289cf-49ba-4b11-8aa8-e229cafdfca8"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("643289cf-49ba-4b11-8aa8-e229cafdfca8"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new CommercialCreateDto
            {
                TitleEn = "01d91f862cc84c3691b4674a2497b25ac111946bc3924f0fb3cb82434376a9836e0f05d670bf4c55a3d",
                TitleAr = "c599855053ec4afa9f19ee9dbdbb35ee08aeab13969d49808",
                PlotNo = "8c3ba9a16fd340e8af17badb2c29c08d8965",
                ActivityEn = "315abfd3",
                ActivityAr = "7178f63b8c6246878bd9d",
                Phone = "4d643901",
                Fax = "dee13736cb944125b9eab795e96a15ac4067442e1c9149cabb41ee024b689a782172fb64a20a47a983f7eae162ff0f",
                IsActive = true,
                Order = 1001083189,
                SubCategoryId = Guid.Parse("79333a96-a172-4e18-8588-0c51d9204eb8")
            };

            // Act
            var serviceResult = await _commercialsAppService.CreateAsync(input);

            // Assert
            var result = await _commercialRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("01d91f862cc84c3691b4674a2497b25ac111946bc3924f0fb3cb82434376a9836e0f05d670bf4c55a3d");
            result.TitleAr.ShouldBe("c599855053ec4afa9f19ee9dbdbb35ee08aeab13969d49808");
            result.PlotNo.ShouldBe("8c3ba9a16fd340e8af17badb2c29c08d8965");
            result.ActivityEn.ShouldBe("315abfd3");
            result.ActivityAr.ShouldBe("7178f63b8c6246878bd9d");
            result.Phone.ShouldBe("4d643901");
            result.Fax.ShouldBe("dee13736cb944125b9eab795e96a15ac4067442e1c9149cabb41ee024b689a782172fb64a20a47a983f7eae162ff0f");
            result.IsActive.ShouldBe(true);
            result.Order.ShouldBe(1001083189);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new CommercialUpdateDto()
            {
                TitleEn = "9bfac601c2234290b03cf5f8f09d51f8ef39ebdbb",
                TitleAr = "950ac5b29b9340d3bd1e4ae15fd7ebfe1445a7d2",
                PlotNo = "2e7748785bb5446bb130944355661168eb",
                ActivityEn = "705e2b40a2d94a62ae21b298833d92d14",
                ActivityAr = "e1bec4217f49458ebe7b52530d1e881d5bad268d051145c2860ef79f3bb230887e153b1800d1445",
                Phone = "96142ccf047246de8cf8d17c44d3d10abdd678a772454c2",
                Fax = "6aacb41be10a4840ad233d2cab5013781d33416aa26342ddb3ea89050c21f1d376f9eb660e8349cfbdcd10dade81e1fc9",
                IsActive = true,
                Order = 496900800,
                SubCategoryId = Guid.Parse("79333a96-a172-4e18-8588-0c51d9204eb8")
            };

            // Act
            var serviceResult = await _commercialsAppService.UpdateAsync(Guid.Parse("643289cf-49ba-4b11-8aa8-e229cafdfca8"), input);

            // Assert
            var result = await _commercialRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.TitleEn.ShouldBe("9bfac601c2234290b03cf5f8f09d51f8ef39ebdbb");
            result.TitleAr.ShouldBe("950ac5b29b9340d3bd1e4ae15fd7ebfe1445a7d2");
            result.PlotNo.ShouldBe("2e7748785bb5446bb130944355661168eb");
            result.ActivityEn.ShouldBe("705e2b40a2d94a62ae21b298833d92d14");
            result.ActivityAr.ShouldBe("e1bec4217f49458ebe7b52530d1e881d5bad268d051145c2860ef79f3bb230887e153b1800d1445");
            result.Phone.ShouldBe("96142ccf047246de8cf8d17c44d3d10abdd678a772454c2");
            result.Fax.ShouldBe("6aacb41be10a4840ad233d2cab5013781d33416aa26342ddb3ea89050c21f1d376f9eb660e8349cfbdcd10dade81e1fc9");
            result.IsActive.ShouldBe(true);
            result.Order.ShouldBe(496900800);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _commercialsAppService.DeleteAsync(Guid.Parse("643289cf-49ba-4b11-8aa8-e229cafdfca8"));

            // Assert
            var result = await _commercialRepository.FindAsync(c => c.Id == Guid.Parse("643289cf-49ba-4b11-8aa8-e229cafdfca8"));

            result.ShouldBeNull();
        }
    }
}