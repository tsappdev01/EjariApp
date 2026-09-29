using DIP.Shared;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.MajorIndustries
{
    public interface IMajorIndustriesAppService : IApplicationService
    {
        Task<PagedResultDto<MajorIndustryWithNavigationPropertiesDto>> GetListAsync(GetMajorIndustriesInput input);

        Task<MajorIndustryWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<MajorIndustryDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetZoneLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<MajorIndustryDto> CreateAsync(MajorIndustryCreateDto input);

        Task<MajorIndustryDto> UpdateAsync(Guid id, MajorIndustryUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(MajorIndustryExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}