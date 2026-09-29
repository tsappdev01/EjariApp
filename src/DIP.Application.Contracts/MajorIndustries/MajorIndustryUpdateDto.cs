using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace DIP.MajorIndustries
{
    public class MajorIndustryUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        public string TitleEn { get; set; }
        [Required]
        public string TitleAr { get; set; }
        public string? Image { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public Guid ZoneId { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}