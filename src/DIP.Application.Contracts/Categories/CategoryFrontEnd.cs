using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.Categories
{
    public class CategoryFrontEnd : FullAuditedEntityDto<Guid>
    {
        public string Title { get; set; }
        public int Order { get; set; }
        public bool IsFeature { get; set; }
        public bool IsActive { get; set; }
    }
}