using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.EFormServices
{
    public partial interface IEFormServicesAppService : IApplicationService
    {
        Task<PagedResultDto<EFormServiceDto>> GetListAsync(GetEFormServicesInput input);

        Task<EFormServiceDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<EFormServiceDto> CreateAsync(EFormServiceCreateDto input);

        Task<EFormServiceDto> UpdateAsync(Guid id, EFormServiceUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(EFormServiceExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}