using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.Zones
{
    public partial interface IZonesAppService : IApplicationService
    {
        Task<PagedResultDto<ZoneDto>> GetListAsync(GetZonesInput input);

        Task<ZoneDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<ZoneDto> CreateAsync(ZoneCreateDto input);

        Task<ZoneDto> UpdateAsync(Guid id, ZoneUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(ZoneExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}