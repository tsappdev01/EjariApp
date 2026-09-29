using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.DipBranches
{
    public class DipBranchDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string? SubTitleEn { get; set; }
        public string? SubTitleAr { get; set; }
        public string? Phone { get; set; }
        public string? AlternativePhone { get; set; }
        public string? Email { get; set; }
        public string? AlternativeEmail { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}