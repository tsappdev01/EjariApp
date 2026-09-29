using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.EFormServiceSubCategories
{
    public class EFormServiceSubCategoryFrontEnd : FullAuditedEntityDto<Guid>
    {
        public string Title { get; set; }
        public string? File { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public Guid EFormServiceId { get; set; }
    }
}