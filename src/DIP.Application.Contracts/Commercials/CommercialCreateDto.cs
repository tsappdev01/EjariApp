using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DIP.Commercials
{
    public class CommercialCreateDto
    {
        [Required]
        public string TitleEn { get; set; }
        [Required]
        public string TitleAr { get; set; }
        public string? PlotNo { get; set; }
        public string? ActivityEn { get; set; }
        public string? ActivityAr { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public string? MakaniNo { get; set; }
        public bool IsActive { get; set; } = true;
        public int Order { get; set; }
        public Guid SubCategoryId { get; set; }
    }
}