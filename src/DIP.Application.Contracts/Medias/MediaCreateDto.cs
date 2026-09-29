using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DIP.Medias
{
    public class MediaCreateDto
    {
        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string? File { get; set; }
        public int Order { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public Guid? ZoneParagraphId { get; set; }
        public Guid? AmenityParagraphId { get; set; }
        public Guid? MediaGalleryId { get; set; }
    }
}