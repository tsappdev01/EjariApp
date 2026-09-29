using DIP.Shared;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.AmenityParagraphs
{
    public interface IAmenityParagraphsAppService : IApplicationService
    {
        Task<PagedResultDto<AmenityParagraphWithNavigationPropertiesDto>> GetListAsync(GetAmenityParagraphsInput input);

        Task<AmenityParagraphWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<AmenityParagraphDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetAmenityLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<AmenityParagraphDto> CreateAsync(AmenityParagraphCreateDto input);

        Task<AmenityParagraphDto> UpdateAsync(Guid id, AmenityParagraphUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(AmenityParagraphExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}