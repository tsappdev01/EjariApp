using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace DIP.TimeLines
{
    public class TimeLineUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        public string TitleEn { get; set; }
        [Required]
        public string TitleAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? Image { get; set; }
        public DateTime? TimeLineDate { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public Guid TimeLineCategoryId { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}