using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.PressReleases
{
    public partial interface IPressReleasesAppService : IApplicationService
    {
        Task<PagedResultDto<PressReleaseDto>> GetListAsync(GetPressReleasesInput input);

        Task<PressReleaseDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<PressReleaseDto> CreateAsync(PressReleaseCreateDto input);

        Task<PressReleaseDto> UpdateAsync(Guid id, PressReleaseUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(PressReleaseExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}