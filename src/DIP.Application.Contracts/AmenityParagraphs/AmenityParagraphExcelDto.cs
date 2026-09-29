using System;

namespace DIP.AmenityParagraphs
{
    public class AmenityParagraphExcelDto
    {
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string? SubTitleEn { get; set; }
        public string? SubTitleAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? ButtonUrl { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}