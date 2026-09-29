using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.EServices
{
    public partial interface IEServicesAppService : IApplicationService
    {
        Task<PagedResultDto<EServiceDto>> GetListAsync(GetEServicesInput input);

        Task<EServiceDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<EServiceDto> CreateAsync(EServiceCreateDto input);

        Task<EServiceDto> UpdateAsync(Guid id, EServiceUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(EServiceExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}