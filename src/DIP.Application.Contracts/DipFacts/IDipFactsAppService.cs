using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.DipFacts
{
    public partial interface IDipFactsAppService : IApplicationService
    {
        Task<PagedResultDto<DipFactDto>> GetListAsync(GetDipFactsInput input);

        Task<DipFactDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<DipFactDto> CreateAsync(DipFactCreateDto input);

        Task<DipFactDto> UpdateAsync(Guid id, DipFactUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(DipFactExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}