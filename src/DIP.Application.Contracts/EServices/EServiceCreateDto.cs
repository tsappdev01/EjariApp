using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DIP.EServices
{
    public class EServiceCreateDto
    {
        [Required]
        public string TitleEn { get; set; }
        public string? TitleAr { get; set; }
        [Required]
        public string Slug { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? Image { get; set; }
        public string? HeaderImage { get; set; }
        public string? MetaTitleEn { get; set; }
        public string? MetaTitleAr { get; set; }
        public string? MetaDescriptionEn { get; set; }
        public string? MetaDescriptionAr { get; set; }
        public int Order { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}