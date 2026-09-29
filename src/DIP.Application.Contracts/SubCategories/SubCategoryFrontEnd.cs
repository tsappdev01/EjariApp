using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.SubCategories
{
    public class SubCategoryFrontEnd : FullAuditedEntityDto<Guid>
    {
        public string? Title { get; set; }
        public int Order { get; set; }
        public bool IsFeature { get; set; }
        public bool IsActive { get; set; }
        public Guid CategoryId { get; set; }
    }
}