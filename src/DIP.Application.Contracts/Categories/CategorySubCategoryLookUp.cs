using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.Categories
{
    public class CategorySubCategoryLookUp : FullAuditedEntityDto<Guid>
    {
        public string Title { get; set; }
        public string ParentTitle { get; set; }
        public int Order { get; set; }
        public bool IsCategory { get; set; }
    }
}