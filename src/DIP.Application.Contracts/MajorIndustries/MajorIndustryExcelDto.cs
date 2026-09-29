using System;

namespace DIP.MajorIndustries
{
    public class MajorIndustryExcelDto
    {
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string? Image { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}