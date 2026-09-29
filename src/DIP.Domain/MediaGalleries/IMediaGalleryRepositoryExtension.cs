using DIP.Amenities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DIP.MediaGalleries
{
    public partial interface IMediaGalleryRepository
    {
        Task<MediaGalleryWithDetails> GetWithDetailsAsync(string slug, CancellationToken cancellationToken = default);
    }
}