using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.MajorIndustries
{
    public class MajorIndustryDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string? Image { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public Guid ZoneId { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}