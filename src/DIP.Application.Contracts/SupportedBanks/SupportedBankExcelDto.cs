using System;

namespace DIP.SupportedBanks
{
    public class SupportedBankExcelDto
    {
        public string? TitleAr { get; set; }
        public string? TitleEn { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
    }
}