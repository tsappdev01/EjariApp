using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.ZoneParagraphs
{
    public class ZoneParagraphDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string? SubTilteEn { get; set; }
        public string? SubTitleAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public Guid ZoneId { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}