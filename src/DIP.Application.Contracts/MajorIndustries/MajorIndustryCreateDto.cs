using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DIP.MajorIndustries
{
    public class MajorIndustryCreateDto
    {
        [Required]
        public string TitleEn { get; set; }
        [Required]
        public string TitleAr { get; set; }
        public string? Image { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid ZoneId { get; set; }
    }
}