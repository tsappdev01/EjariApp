using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.LastEventss
{
    public partial interface ILastEventssAppService : IApplicationService
    {
        Task<PagedResultDto<LastEventsDto>> GetListAsync(GetLastEventssInput input);

        Task<LastEventsDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<LastEventsDto> CreateAsync(LastEventsCreateDto input);

        Task<LastEventsDto> UpdateAsync(Guid id, LastEventsUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(LastEventsExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}