using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace DIP.SliderHomePages
{
    public class SliderHomePageUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        public string TitleAr { get; set; } = null!;
        [Required]
        public string TitleEn { get; set; } = null!;
        [Required]
        public string DescriptionAr { get; set; } = null!;
        [Required]
        public string DescriptionEn { get; set; } = null!;
        public string? ButtonTitleAr { get; set; }
        public string? ButtonTitleEn { get; set; }
        public string? ButtonUrlEn { get; set; }
        public string? ButtonUrlAr { get; set; }
        public string? Image { get; set; }
        public string? YoutubeUrl { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}