using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;
using System.Collections.Generic;

namespace DIP.SupportedBanks
{
    public interface ISupportedBanksAppService : IApplicationService
    {
        Task<PagedResultDto<SupportedBankDto>> GetListAsync(GetSupportedBanksInput input);

        Task<SupportedBankDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<SupportedBankDto> CreateAsync(SupportedBankCreateDto input);

        Task<SupportedBankDto> UpdateAsync(Guid id, SupportedBankUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(SupportedBankExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();

        Task<List<SupportedBankFrontEnd>> GetListFrontEndAsync(GetSupportedBanksInput input);
    }
}