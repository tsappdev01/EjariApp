using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace DIP.EFormServiceSubCategories
{
    public class EFormServiceSubCategoryUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        public string TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string? File { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public Guid EFormServiceId { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}