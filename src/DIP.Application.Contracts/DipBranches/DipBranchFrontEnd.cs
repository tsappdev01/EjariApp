using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.DipBranches
{
    public class DipBranchFront : FullAuditedEntityDto<Guid>
    {
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public string? Phone { get; set; }
        public string? AlternativePhone { get; set; }
        public string? Email { get; set; }
        public string? AlternativeEmail { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}