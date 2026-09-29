using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace DIP.AmenityParagraphs
{
    public class AmenityParagraphUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        public string TitleEn { get; set; }
        [Required]
        public string TitleAr { get; set; }
        public string? SubTitleEn { get; set; }
        public string? SubTitleAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? ButtonUrl { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public Guid AmenityId { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}