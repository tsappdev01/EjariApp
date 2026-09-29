using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.EServices
{
    public class EServiceFrontEnd : FullAuditedEntityDto<Guid>
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? HeaderImage { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}