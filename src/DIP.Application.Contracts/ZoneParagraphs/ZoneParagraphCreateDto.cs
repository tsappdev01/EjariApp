using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DIP.ZoneParagraphs
{
    public class ZoneParagraphCreateDto
    {
        [Required]
        public string TitleEn { get; set; }
        [Required]
        public string TitleAr { get; set; }
        public string? SubTilteEn { get; set; }
        public string? SubTitleAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public int Order { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public Guid ZoneId { get; set; }
    }
}