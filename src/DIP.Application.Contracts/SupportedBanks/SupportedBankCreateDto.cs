using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DIP.SupportedBanks
{
    public class SupportedBankCreateDto
    {
        public string? TitleAr { get; set; }
        public string? TitleEn { get; set; }
        public bool IsActive { get; set; } = true;
        public int Order { get; set; }
    }
}