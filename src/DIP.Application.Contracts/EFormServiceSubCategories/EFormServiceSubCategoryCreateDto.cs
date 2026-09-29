using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DIP.EFormServiceSubCategories
{
    public class EFormServiceSubCategoryCreateDto
    {
        [Required]
        public string TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string? File { get; set; }
        public int Order { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public Guid EFormServiceId { get; set; }
    }
}