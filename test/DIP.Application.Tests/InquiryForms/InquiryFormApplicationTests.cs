using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.InquiryForms
{
    public class InquiryFormsAppServiceTests : DIPApplicationTestBase
    {
        private readonly IInquiryFormsAppService _inquiryFormsAppService;
        private readonly IRepository<InquiryForm, Guid> _inquiryFormRepository;

        public InquiryFormsAppServiceTests()
        {
            _inquiryFormsAppService = GetRequiredService<IInquiryFormsAppService>();
            _inquiryFormRepository = GetRequiredService<IRepository<InquiryForm, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _inquiryFormsAppService.GetListAsync(new GetInquiryFormsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("0282fa84-8efb-48fd-9818-684f3e2056ec")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("bc148693-7979-43f6-bfe0-9067750764f5")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _inquiryFormsAppService.GetAsync(Guid.Parse("0282fa84-8efb-48fd-9818-684f3e2056ec"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("0282fa84-8efb-48fd-9818-684f3e2056ec"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new InquiryFormCreateDto
            {
                CompanyName = "f7ba7a4f10be4ed7a3e43ceb2e0fefb3b13a8ff6d1ff499293a1",
                Name = "c45f0aaf120c442c85f755578e15755b226c97762a59450",
                Email = "85d7075628@9b087e9e55.com",
                Mobile = "ccc615c44f564b54a77e1231ff36adc1c8eb07273ac246598fc9345a3a7",
                Fax = "04df9be23f0b4385866fe240d9abf9724a200055721d43eba66ad1d6a8077f11f116ed4190f84a98bd5b10f5",
                Phone = "6f3ae9efaeb14792a97aaa",
                InquiryType = "831b81be0b714f918da1f80252af0931b1ce435a0ebc41de941cf7b11a28e85398a9de4f6d4",
                TradeLicensePlateOfIssue = "d19588c03ba9",
                BuyRent = "93ed76fade97461fb709f2bff4db595872aad740786249a5a11182fc1a02199380ee8c619e4d47e5a65bd643e00472",
                SpaceInSquareFeet = 1156715747,
                Comments = "12f00408a40d43488e4104c1c967ed"
            };

            // Act
            var serviceResult = await _inquiryFormsAppService.CreateAsync(input);

            // Assert
            var result = await _inquiryFormRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.CompanyName.ShouldBe("f7ba7a4f10be4ed7a3e43ceb2e0fefb3b13a8ff6d1ff499293a1");
            result.Name.ShouldBe("c45f0aaf120c442c85f755578e15755b226c97762a59450");
            result.Email.ShouldBe("85d7075628@9b087e9e55.com");
            result.Mobile.ShouldBe("ccc615c44f564b54a77e1231ff36adc1c8eb07273ac246598fc9345a3a7");
            result.Fax.ShouldBe("04df9be23f0b4385866fe240d9abf9724a200055721d43eba66ad1d6a8077f11f116ed4190f84a98bd5b10f5");
            result.Phone.ShouldBe("6f3ae9efaeb14792a97aaa");
            result.InquiryType.ShouldBe("831b81be0b714f918da1f80252af0931b1ce435a0ebc41de941cf7b11a28e85398a9de4f6d4");
            result.TradeLicensePlateOfIssue.ShouldBe("d19588c03ba9");
            result.BuyRent.ShouldBe("93ed76fade97461fb709f2bff4db595872aad740786249a5a11182fc1a02199380ee8c619e4d47e5a65bd643e00472");
            result.SpaceInSquareFeet.ShouldBe(1156715747);
            result.Comments.ShouldBe("12f00408a40d43488e4104c1c967ed");
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new InquiryFormUpdateDto()
            {
                CompanyName = "d79c83deda664e8cbd08e754e57ce7258898eebf14ef42bd97d916a6809d0edcba92cc07",
                Name = "9b2098ad",
                Email = "@.com",
                Mobile = "3ae12280b8594bf08007a3c",
                Fax = "0476e746d88445f4a28ed450ca0e5c98505e915c83f94eb7904c9e3be3cc1edc861dd4be08b24b53a0e95521747a6d2dd",
                Phone = "1257b9a7e0304a56a3daef8e14e61b54e9a2f183952541588893122ef099d2a1f58dcd8c49b5",
                InquiryType = "55b12cdbfb534",
                TradeLicensePlateOfIssue = "73afe9d20e25443bbc5d98892f517e5713dc1d57d0e944258c26e3e3623d1955025a032206ae4787816256c95e",
                BuyRent = "752cf1bf5fda46c78ce657720",
                SpaceInSquareFeet = 1293990908,
                Comments = "22677b451d0a451490bda2b0df8bcf4b43420ca657ae4cb6a8ecfe4b53ca16dd10d64b"
            };

            // Act
            var serviceResult = await _inquiryFormsAppService.UpdateAsync(Guid.Parse("0282fa84-8efb-48fd-9818-684f3e2056ec"), input);

            // Assert
            var result = await _inquiryFormRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.CompanyName.ShouldBe("d79c83deda664e8cbd08e754e57ce7258898eebf14ef42bd97d916a6809d0edcba92cc07");
            result.Name.ShouldBe("9b2098ad");
            result.Email.ShouldBe("@.com");
            result.Mobile.ShouldBe("3ae12280b8594bf08007a3c");
            result.Fax.ShouldBe("0476e746d88445f4a28ed450ca0e5c98505e915c83f94eb7904c9e3be3cc1edc861dd4be08b24b53a0e95521747a6d2dd");
            result.Phone.ShouldBe("1257b9a7e0304a56a3daef8e14e61b54e9a2f183952541588893122ef099d2a1f58dcd8c49b5");
            result.InquiryType.ShouldBe("55b12cdbfb534");
            result.TradeLicensePlateOfIssue.ShouldBe("73afe9d20e25443bbc5d98892f517e5713dc1d57d0e944258c26e3e3623d1955025a032206ae4787816256c95e");
            result.BuyRent.ShouldBe("752cf1bf5fda46c78ce657720");
            result.SpaceInSquareFeet.ShouldBe(1293990908);
            result.Comments.ShouldBe("22677b451d0a451490bda2b0df8bcf4b43420ca657ae4cb6a8ecfe4b53ca16dd10d64b");
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _inquiryFormsAppService.DeleteAsync(Guid.Parse("0282fa84-8efb-48fd-9818-684f3e2056ec"));

            // Assert
            var result = await _inquiryFormRepository.FindAsync(c => c.Id == Guid.Parse("0282fa84-8efb-48fd-9818-684f3e2056ec"));

            result.ShouldBeNull();
        }
    }
}