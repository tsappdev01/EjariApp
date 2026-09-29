using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.MajorIndustries
{
    public class MajorIndustryFrontEnd : FullAuditedEntityDto<Guid>
    {
        public string? Title { get; set; }
        public string? Image { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public Guid ZoneId { get; set; }
    }
}