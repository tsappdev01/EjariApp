using System;

namespace DIP.ZoneParagraphs
{
    public class ZoneParagraphExcelDto
    {
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string? SubTilteEn { get; set; }
        public string? SubTitleAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}