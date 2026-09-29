using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.PageInfos
{
    public partial interface IPageInfosAppService : IApplicationService
    {
        Task<PagedResultDto<PageInfoDto>> GetListAsync(GetPageInfosInput input);

        Task<PageInfoDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<PageInfoDto> CreateAsync(PageInfoCreateDto input);

        Task<PageInfoDto> UpdateAsync(Guid id, PageInfoUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(PageInfoExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}