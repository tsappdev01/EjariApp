using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.SupportedBanks
{
    public class SupportedBankFrontEnd : FullAuditedEntityDto<Guid>
    {
        public string Title { get; set; }
        public int Order { get; set; }
 
        public bool IsActive { get; set; }
    }
}