using DIP.Shared;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.SubCategories
{
    public partial interface ISubCategoriesAppService : IApplicationService
    {
        Task<PagedResultDto<SubCategoryWithNavigationPropertiesDto>> GetListAsync(GetSubCategoriesInput input);

        Task<SubCategoryWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<SubCategoryDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetCategoryLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<SubCategoryDto> CreateAsync(SubCategoryCreateDto input);

        Task<SubCategoryDto> UpdateAsync(Guid id, SubCategoryUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(SubCategoryExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}