using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.LastEventss
{
    public class LastEventsFrontEnd : FullAuditedEntityDto<Guid>
    {
        public string? Title { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public bool IsFeatured { get; set; }
        public string? Slug { get; set; }
        public string? Image { get; set; }
        public string? HeaderImage { get; set; }
        public string? Description { get; set; }
        public string? Summary { get; set; }
        public int Order { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public string? Location { get; set; }
    }
}