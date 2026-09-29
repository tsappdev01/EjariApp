using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.TimeLines
{
    public class TimeLineDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string TitleEn { get; set; }
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