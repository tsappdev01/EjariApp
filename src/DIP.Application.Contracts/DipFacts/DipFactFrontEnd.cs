using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.DipFacts
{
    public class DipFactFrontEnd : FullAuditedEntityDto<Guid>
    {
        public string? Image { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}