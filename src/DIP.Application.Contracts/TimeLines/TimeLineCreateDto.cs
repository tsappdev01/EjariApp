using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DIP.TimeLines
{
    public class TimeLineCreateDto
    {
        [Required]
        public string TitleEn { get; set; }
        [Required]
        public string TitleAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? Image { get; set; }
        public DateTime? TimeLineDate { get; set; }
        public int Order { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public Guid TimeLineCategoryId { get; set; }
    }
}