using DIP.Shared;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.EFormServiceSubCategories
{
    public interface IEFormServiceSubCategoriesAppService : IApplicationService
    {
        Task<PagedResultDto<EFormServiceSubCategoryWithNavigationPropertiesDto>> GetListAsync(GetEFormServiceSubCategoriesInput input);

        Task<EFormServiceSubCategoryWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<EFormServiceSubCategoryDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetEFormServiceLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<EFormServiceSubCategoryDto> CreateAsync(EFormServiceSubCategoryCreateDto input);

        Task<EFormServiceSubCategoryDto> UpdateAsync(Guid id, EFormServiceSubCategoryUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(EFormServiceSubCategoryExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}