using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace DIP.Categories
{
    public class CategoryUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        public string TitleAr { get; set; }
        [Required]
        public string TitleEn { get; set; }
        public int Order { get; set; }
        public bool IsFeature { get; set; }
        public bool IsActive { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}