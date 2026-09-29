using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.SiteSettings
{
    public partial interface ISiteSettingsAppService : IApplicationService
    {
        Task<PagedResultDto<SiteSettingDto>> GetListAsync(GetSiteSettingsInput input);

        Task<SiteSettingDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<SiteSettingDto> CreateAsync(SiteSettingCreateDto input);

        Task<SiteSettingDto> UpdateAsync(Guid id, SiteSettingUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(SiteSettingExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}