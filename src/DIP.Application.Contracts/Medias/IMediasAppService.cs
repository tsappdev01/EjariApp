using DIP.Shared;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;
using System.Collections.Generic;

namespace DIP.Medias
{
    public partial interface IMediasAppService : IApplicationService
    {
        Task<PagedResultDto<MediaWithNavigationPropertiesDto>> GetListAsync(GetMediasInput input);

        Task<MediaWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<MediaDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetZoneParagraphLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetAmenityParagraphLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetMediaGalleryLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<MediaDto> CreateAsync(MediaCreateDto input);

        Task<MediaDto> UpdateAsync(Guid id, MediaUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(MediaExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}