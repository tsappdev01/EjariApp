using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DIP.TimeLineCategories
{
    public class TimeLineCategoryCreateDto
    {
        [Required]
        public string TitleEn { get; set; }
        [Required]
        public string TitleAr { get; set; }
        public int Order { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}