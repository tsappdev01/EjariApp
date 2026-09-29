using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.MediaGalleries
{
    public partial interface IMediaGalleriesAppService : IApplicationService
    {
        Task<PagedResultDto<MediaGalleryDto>> GetListAsync(GetMediaGalleriesInput input);

        Task<MediaGalleryDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<MediaGalleryDto> CreateAsync(MediaGalleryCreateDto input);

        Task<MediaGalleryDto> UpdateAsync(Guid id, MediaGalleryUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(MediaGalleryExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}