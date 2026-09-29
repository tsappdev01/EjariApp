using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.TimeLineCategories
{
    public partial interface ITimeLineCategoriesAppService : IApplicationService
    {
        Task<PagedResultDto<TimeLineCategoryDto>> GetListAsync(GetTimeLineCategoriesInput input);

        Task<TimeLineCategoryDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<TimeLineCategoryDto> CreateAsync(TimeLineCategoryCreateDto input);

        Task<TimeLineCategoryDto> UpdateAsync(Guid id, TimeLineCategoryUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(TimeLineCategoryExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}