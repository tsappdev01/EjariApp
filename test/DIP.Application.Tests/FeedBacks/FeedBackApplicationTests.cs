using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DIP.FeedBacks
{
    public class FeedBacksAppServiceTests : DIPApplicationTestBase
    {
        private readonly IFeedBacksAppService _feedBacksAppService;
        private readonly IRepository<FeedBack, Guid> _feedBackRepository;

        public FeedBacksAppServiceTests()
        {
            _feedBacksAppService = GetRequiredService<IFeedBacksAppService>();
            _feedBackRepository = GetRequiredService<IRepository<FeedBack, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _feedBacksAppService.GetListAsync(new GetFeedBacksInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("0df1f9a1-94f5-4dd8-b366-7907cc36c9a3")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("cb96bb9d-c5cb-45c3-9b6f-e93d71991d0f")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _feedBacksAppService.GetAsync(Guid.Parse("0df1f9a1-94f5-4dd8-b366-7907cc36c9a3"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("0df1f9a1-94f5-4dd8-b366-7907cc36c9a3"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new FeedBackCreateDto
            {
                Subject = "a35c837eb1f945399e1a31ab570304bb3023d0411740488590",
                CompanyName = "29bf3da4da",
                PlotNo = "1c10a5a7e29c44c2be1d80a64f974d1045e08ad2bef94086b78555e7c34a484addc",
                PlotCategory = "b8c491f92eb349eba48bc3652",
                ContactPersonName = "bd328b847eee4e588d57482a9ac3915848539ed91bbc49d88ce99ffe922b584da961ca11d946443fb204dee2",
                EmailId = "d36e735932a24b68a00727c5036728cdaf773c177@dd723040a668472abd547654168546b5ccd63d10e.com",
                MobileNumber = "f08cb3575c144d95bab888d9ada526b25d7b8",
                Department = "18144bb0d7fe4c0c8d035dad745f7dc0037746cb91f64ce6a2be100d33b8b4da57d8e3bca9a445",
                CategoryName = "72fad4391ad540838625629e44d2aebfc346754e4c3d41",
                Description = "e402a3ea9e5f497da47da6bfeedd2c7ae1e4e4c66d394fbe8d03c74194f3c4ac9f031f5e38e54f58a33530dec92d109dc0d"
            };

            // Act
            var serviceResult = await _feedBacksAppService.CreateAsync(input);

            // Assert
            var result = await _feedBackRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Subject.ShouldBe("a35c837eb1f945399e1a31ab570304bb3023d0411740488590");
            result.CompanyName.ShouldBe("29bf3da4da");
            result.PlotNo.ShouldBe("1c10a5a7e29c44c2be1d80a64f974d1045e08ad2bef94086b78555e7c34a484addc");
            result.PlotCategory.ShouldBe("b8c491f92eb349eba48bc3652");
            result.ContactPersonName.ShouldBe("bd328b847eee4e588d57482a9ac3915848539ed91bbc49d88ce99ffe922b584da961ca11d946443fb204dee2");
            result.EmailId.ShouldBe("d36e735932a24b68a00727c5036728cdaf773c177@dd723040a668472abd547654168546b5ccd63d10e.com");
            result.MobileNumber.ShouldBe("f08cb3575c144d95bab888d9ada526b25d7b8");
            result.Department.ShouldBe("18144bb0d7fe4c0c8d035dad745f7dc0037746cb91f64ce6a2be100d33b8b4da57d8e3bca9a445");
            result.CategoryName.ShouldBe("72fad4391ad540838625629e44d2aebfc346754e4c3d41");
            result.Description.ShouldBe("e402a3ea9e5f497da47da6bfeedd2c7ae1e4e4c66d394fbe8d03c74194f3c4ac9f031f5e38e54f58a33530dec92d109dc0d");
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new FeedBackUpdateDto()
            {
                Subject = "16bf35ef45824121b91d03133eed3aa868c05b4acefa431d9f02255630755b068ceb58f4d62a40cbb97874d89033967",
                CompanyName = "cde376a1bd4b4f19b5df0fa6e87aec8044abccc860",
                PlotNo = "7221a530aee543cfaf2f47bf51bf4df52d1f421fd9ea40bcaeae3302769236e16c9d",
                PlotCategory = "0edd2bc0cd9047d284757ab69e13b723372a0db339b94a",
                ContactPersonName = "b7bbf6ae836c4528bb20dd08b39fb141e8fc46655f5d40db9a613f2b054b8977cf81ee7969ee4bda9ae81d",
                EmailId = "2eb08@867be.com",
                MobileNumber = "a0465c15f49c45a0aaf7feb884d3274f8f94f3fe4ff84c2cad76b560dd725ed56b",
                Department = "6f6231ddfa284e45b63c4ce16dda1ffd2338b45578bb420593a5f14497b5ac2338d81",
                CategoryName = "647a9ed0f1d84839aea1c50846c6e47f5faf7c",
                Description = "e01a55"
            };

            // Act
            var serviceResult = await _feedBacksAppService.UpdateAsync(Guid.Parse("0df1f9a1-94f5-4dd8-b366-7907cc36c9a3"), input);

            // Assert
            var result = await _feedBackRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Subject.ShouldBe("16bf35ef45824121b91d03133eed3aa868c05b4acefa431d9f02255630755b068ceb58f4d62a40cbb97874d89033967");
            result.CompanyName.ShouldBe("cde376a1bd4b4f19b5df0fa6e87aec8044abccc860");
            result.PlotNo.ShouldBe("7221a530aee543cfaf2f47bf51bf4df52d1f421fd9ea40bcaeae3302769236e16c9d");
            result.PlotCategory.ShouldBe("0edd2bc0cd9047d284757ab69e13b723372a0db339b94a");
            result.ContactPersonName.ShouldBe("b7bbf6ae836c4528bb20dd08b39fb141e8fc46655f5d40db9a613f2b054b8977cf81ee7969ee4bda9ae81d");
            result.EmailId.ShouldBe("2eb08@867be.com");
            result.MobileNumber.ShouldBe("a0465c15f49c45a0aaf7feb884d3274f8f94f3fe4ff84c2cad76b560dd725ed56b");
            result.Department.ShouldBe("6f6231ddfa284e45b63c4ce16dda1ffd2338b45578bb420593a5f14497b5ac2338d81");
            result.CategoryName.ShouldBe("647a9ed0f1d84839aea1c50846c6e47f5faf7c");
            result.Description.ShouldBe("e01a55");
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _feedBacksAppService.DeleteAsync(Guid.Parse("0df1f9a1-94f5-4dd8-b366-7907cc36c9a3"));

            // Assert
            var result = await _feedBackRepository.FindAsync(c => c.Id == Guid.Parse("0df1f9a1-94f5-4dd8-b366-7907cc36c9a3"));

            result.ShouldBeNull();
        }
    }
}