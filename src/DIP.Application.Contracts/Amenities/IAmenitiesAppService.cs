using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.Amenities
{
    public partial interface IAmenitiesAppService : IApplicationService
    {
        Task<PagedResultDto<AmenityDto>> GetListAsync(GetAmenitiesInput input);

        Task<AmenityDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<AmenityDto> CreateAsync(AmenityCreateDto input);

        Task<AmenityDto> UpdateAsync(Guid id, AmenityUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(AmenityExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}