using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.Commercials
{
    public class CommercialFrontEnd : FullAuditedEntityDto<Guid>
    {
        public string? Title { get; set; }
        public string? PlotNo { get; set; }
        public string? Activity { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public string? MakaniNo { get; set; }
        public string? Category { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
        public Guid SubCategoryId { get; set; }
    }
}