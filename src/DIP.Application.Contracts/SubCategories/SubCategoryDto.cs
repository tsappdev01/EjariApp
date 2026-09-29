using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.SubCategories
{
    public class SubCategoryDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public int Order { get; set; }
        public bool IsFeature { get; set; }
        public bool IsActive { get; set; }
        public Guid CategoryId { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}