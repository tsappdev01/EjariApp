using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.DipBranches
{
    public partial interface IDipBranchesAppService : IApplicationService
    {
        Task<PagedResultDto<DipBranchDto>> GetListAsync(GetDipBranchesInput input);

        Task<DipBranchDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<DipBranchDto> CreateAsync(DipBranchCreateDto input);

        Task<DipBranchDto> UpdateAsync(Guid id, DipBranchUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(DipBranchExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}