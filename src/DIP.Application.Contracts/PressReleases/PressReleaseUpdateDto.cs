using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace DIP.PressReleases
{
    public class PressReleaseUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        public string TitleAr { get; set; }
        [Required]
        public string TitleEn { get; set; }
        public string? MetaTitleAr { get; set; }
        public string? MetaTitleEn { get; set; }
        public string? MetaDescriptionEn { get; set; }
        public string? MetaDescriptionAr { get; set; }
        public bool IsFeatured { get; set; }
        [Required]
        public string Slug { get; set; }
        public string? Image { get; set; }
        public string? HeaderImage { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        [Required]
        public string SummaryEn { get; set; }
        [Required]
        public string SummaryAr { get; set; }
        public int Order { get; set; }
        public DateTime Date { get; set; }
        public bool IsActive { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}