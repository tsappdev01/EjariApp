using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.ContactForms
{
    public class ContactFormsAppServiceTests : DIPApplicationTestBase
    {
        private readonly IContactFormsAppService _contactFormsAppService;
        private readonly IRepository<ContactForm, Guid> _contactFormRepository;

        public ContactFormsAppServiceTests()
        {
            _contactFormsAppService = GetRequiredService<IContactFormsAppService>();
            _contactFormRepository = GetRequiredService<IRepository<ContactForm, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _contactFormsAppService.GetListAsync(new GetContactFormsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("d7dd5e88-358c-4746-a8e7-0df1a50e5350")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("ab8a51d8-4d65-4fb3-ada1-47d0fc6eff9a")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _contactFormsAppService.GetAsync(Guid.Parse("d7dd5e88-358c-4746-a8e7-0df1a50e5350"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("d7dd5e88-358c-4746-a8e7-0df1a50e5350"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new ContactFormCreateDto
            {
                FullName = "743a18190fcc4efbb24ea7767733cea3a02444cfec264abcb6d5bbfea110de17a5e2097efa6b4d3eaabcccb50c",
                Email = "72696dee6c5f442295b@51f4d18cd45a4f95828.com",
                Subject = "4114a1b2859340b6b4597ba2b913a7595161",
                Message = "dac23752e440422abed3471498d689ab494531aae7b6465099c505840e3c6bda3f1e34665f304333bb48413160b35"
            };

            // Act
            var serviceResult = await _contactFormsAppService.CreateAsync(input);

            // Assert
            var result = await _contactFormRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.FullName.ShouldBe("743a18190fcc4efbb24ea7767733cea3a02444cfec264abcb6d5bbfea110de17a5e2097efa6b4d3eaabcccb50c");
            result.Email.ShouldBe("72696dee6c5f442295b@51f4d18cd45a4f95828.com");
            result.Subject.ShouldBe("4114a1b2859340b6b4597ba2b913a7595161");
            result.Message.ShouldBe("dac23752e440422abed3471498d689ab494531aae7b6465099c505840e3c6bda3f1e34665f304333bb48413160b35");
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new ContactFormUpdateDto()
            {
                FullName = "c55e9fe25bc84067a510a3c8ef42c73f251989ce3e9a48d5b891970066384703542980c2711a4219932f296f919df72d95f",
                Email = "a1cdbf4a4cbf46eba89b00b05ea25@67c93916bad14d3ea73b2cbb10c32.com",
                Subject = "b7d2226a5d674f57a0bf6b71a0c7f4de294ba94a6c6a42ed92c5f7",
                Message = "52cf5a5"
            };

            // Act
            var serviceResult = await _contactFormsAppService.UpdateAsync(Guid.Parse("d7dd5e88-358c-4746-a8e7-0df1a50e5350"), input);

            // Assert
            var result = await _contactFormRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.FullName.ShouldBe("c55e9fe25bc84067a510a3c8ef42c73f251989ce3e9a48d5b891970066384703542980c2711a4219932f296f919df72d95f");
            result.Email.ShouldBe("a1cdbf4a4cbf46eba89b00b05ea25@67c93916bad14d3ea73b2cbb10c32.com");
            result.Subject.ShouldBe("b7d2226a5d674f57a0bf6b71a0c7f4de294ba94a6c6a42ed92c5f7");
            result.Message.ShouldBe("52cf5a5");
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _contactFormsAppService.DeleteAsync(Guid.Parse("d7dd5e88-358c-4746-a8e7-0df1a50e5350"));

            // Assert
            var result = await _contactFormRepository.FindAsync(c => c.Id == Guid.Parse("d7dd5e88-358c-4746-a8e7-0df1a50e5350"));

            result.ShouldBeNull();
        }
    }
}