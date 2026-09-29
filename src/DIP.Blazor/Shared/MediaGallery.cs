using DIP.Medias;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DIP.Blazor.Shared
{
    public class MediaGallery
    {
        public List<MediaCreateDto> NewMedias { get; set; }
        public List<MediaUpdateDto> EditingMedias { get; set; }

        [JsonConstructor]
    public MediaGallery()
    {
            NewMedias = new List<MediaCreateDto>();
            EditingMedias = new List<MediaUpdateDto>();
    }
}
}
