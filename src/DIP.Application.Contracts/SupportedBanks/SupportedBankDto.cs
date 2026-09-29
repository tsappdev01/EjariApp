using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.SupportedBanks
{
    public class SupportedBankDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string? TitleAr { get; set; }
        public string? TitleEn { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}