using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DIP.AmenityParagraphs
{
    public class AmenityParagraphCreateDto
    {
        [Required]
        public string TitleEn { get; set; }
        [Required]
        public string TitleAr { get; set; }
        public string? SubTitleEn { get; set; }
        public string? SubTitleAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? ButtonUrl { get; set; }
        public int Order { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public Guid AmenityId { get; set; }
    }
}