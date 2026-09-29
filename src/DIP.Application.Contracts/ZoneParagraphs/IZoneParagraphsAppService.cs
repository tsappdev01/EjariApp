using DIP.Shared;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.ZoneParagraphs
{
    public interface IZoneParagraphsAppService : IApplicationService
    {
        Task<PagedResultDto<ZoneParagraphWithNavigationPropertiesDto>> GetListAsync(GetZoneParagraphsInput input);

        Task<ZoneParagraphWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<ZoneParagraphDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetZoneLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<ZoneParagraphDto> CreateAsync(ZoneParagraphCreateDto input);

        Task<ZoneParagraphDto> UpdateAsync(Guid id, ZoneParagraphUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(ZoneParagraphExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}