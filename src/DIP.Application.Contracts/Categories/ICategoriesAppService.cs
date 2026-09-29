using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.Categories
{
    public partial interface ICategoriesAppService : IApplicationService
    {
        Task<PagedResultDto<CategoryDto>> GetListAsync(GetCategoriesInput input);

        Task<CategoryDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<CategoryDto> CreateAsync(CategoryCreateDto input);

        Task<CategoryDto> UpdateAsync(Guid id, CategoryUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(CategoryExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}