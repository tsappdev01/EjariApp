using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace DIP.MediaGalleries
{
    public partial interface IMediaGalleriesAppService
    {
        Task<MediaGalleryFrontEnd> GetWithDetailsFrontEndAsync(string slug);
        Task<PagedResultDto<MediaGalleryFrontEnd>> GetListFrontEndAsync(GetMediaGalleriesInput input);
    }
}