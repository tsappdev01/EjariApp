using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DIP.SubCategories
{
    public class SubCategoryCreateDto
    {
        [Required]
        public string TitleAr { get; set; }
        [Required]
        public string TitleEn { get; set; }
        public int Order { get; set; }
        public bool IsFeature { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public Guid CategoryId { get; set; }
    }
}