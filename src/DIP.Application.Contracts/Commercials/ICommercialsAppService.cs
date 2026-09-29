using DIP.Shared;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.Commercials
{
    public partial interface ICommercialsAppService : IApplicationService
    {
        Task<PagedResultDto<CommercialWithNavigationPropertiesDto>> GetListAsync(GetCommercialsInput input);

        Task<CommercialWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<CommercialDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetSubCategoryLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<CommercialDto> CreateAsync(CommercialCreateDto input);

        Task<CommercialDto> UpdateAsync(Guid id, CommercialUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(CommercialExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}