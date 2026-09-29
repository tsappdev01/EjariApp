using System;

namespace DIP.DipFacts
{
    public class DipFactExcelDto
    {
        public string? Image { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}