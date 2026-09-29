using DIP.EFormServiceSubCategories;
using DIP.ZoneParagraphs;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.EFormServices
{
    public class EFormServiceFrontEnd : FullAuditedEntityDto<Guid>
    {
        public string Title { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public List<EFormServiceSubCategoryFrontEnd> EFormServiceSubCategories { get; set; }
    }
}