using DIP.TimeLines;
using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.TimeLines
{
    public class TimeLineFrontEnd : FullAuditedEntityDto<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public DateTime? TimeLineDate { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public Guid TimeLineCategoryId { get; set; }
    }
}