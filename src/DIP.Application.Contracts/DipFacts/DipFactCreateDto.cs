using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DIP.DipFacts
{
    public class DipFactCreateDto
    {
        public string? Image { get; set; }
        [Required]
        public string TitleAr { get; set; }
        [Required]
        public string TitleEn { get; set; }
        [Required]
        public string DescriptionAr { get; set; }
        [Required]
        public string DescriptionEn { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}