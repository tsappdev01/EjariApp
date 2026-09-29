using DIP.Amenities;
using DIP.ZoneParagraphs;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace DIP.MediaGalleries
{
    public partial class EfCoreMediaGalleryRepository
    {
        public async Task<MediaGalleryWithDetails> GetWithDetailsAsync(string slug, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return (await GetDbSetAsync()).Where(a => a.IsActive && a.Slug == slug)
                .Select(mediaGallery => new MediaGalleryWithDetails
                {
                    MediaGallery = mediaGallery,
                    Medias = dbContext.Medias.Where(b => b.IsActive && b.MediaGalleryId == mediaGallery.Id).OrderBy(m => m.Order).ToList()
                }).FirstOrDefault();
        }
    }
}