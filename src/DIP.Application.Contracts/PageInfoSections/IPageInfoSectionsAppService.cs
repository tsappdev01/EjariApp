using DIP.Shared;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.PageInfoSections
{
    public interface IPageInfoSectionsAppService : IApplicationService
    {
        Task<PagedResultDto<PageInfoSectionWithNavigationPropertiesDto>> GetListAsync(GetPageInfoSectionsInput input);

        Task<PageInfoSectionWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<PageInfoSectionDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetPageInfoLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<PageInfoSectionDto> CreateAsync(PageInfoSectionCreateDto input);

        Task<PageInfoSectionDto> UpdateAsync(Guid id, PageInfoSectionUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(PageInfoSectionExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}