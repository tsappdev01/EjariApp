using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DIP.EFormServices
{
    public class EFormServiceCreateDto
    {
        [Required]
        public string TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public int Order { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}