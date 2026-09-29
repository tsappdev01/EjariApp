using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace DIP.SupportedBanks
{
    public class SupportedBankUpdateDto : IHasConcurrencyStamp
    {
        public string? TitleAr { get; set; }
        public string? TitleEn { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}