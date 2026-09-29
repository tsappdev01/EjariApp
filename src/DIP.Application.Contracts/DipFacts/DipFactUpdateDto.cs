using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace DIP.DipFacts
{
    public class DipFactUpdateDto : IHasConcurrencyStamp
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

        public string ConcurrencyStamp { get; set; }
    }
}