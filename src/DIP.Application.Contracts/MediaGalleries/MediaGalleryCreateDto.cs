using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DIP.MediaGalleries
{
    public class MediaGalleryCreateDto
    {
        [Required]
        public string TitleEn { get; set; }
        [Required]
        public string Slug { get; set; }
        public string? TitleAr { get; set; }
        public string? MetaTitleEn { get; set; }
        public string? MetaTitleAr { get; set; }
        public string? MetaDescriptionEn { get; set; }
        public string? MetaDescriptionAr { get; set; }
        public string? SummaryEn { get; set; }
        public string? SummaryAr { get; set; }
        public string? HeaderImage { get; set; }
        public string? Image { get; set; }
        public int Order { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}