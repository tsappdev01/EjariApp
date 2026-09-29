using DIP.Shared;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.TimeLines
{
    public interface ITimeLinesAppService : IApplicationService
    {
        Task<PagedResultDto<TimeLineWithNavigationPropertiesDto>> GetListAsync(GetTimeLinesInput input);

        Task<TimeLineWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<TimeLineDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetTimeLineCategoryLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<TimeLineDto> CreateAsync(TimeLineCreateDto input);

        Task<TimeLineDto> UpdateAsync(Guid id, TimeLineUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(TimeLineExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}