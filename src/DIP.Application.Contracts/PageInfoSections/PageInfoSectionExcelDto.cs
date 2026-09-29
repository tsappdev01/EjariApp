using System;

namespace DIP.PageInfoSections
{
    public class PageInfoSectionExcelDto
    {
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string? SubTitleEn { get; set; }
        public string? SubTitleAr { get; set; }
        public string? SummaryEn { get; set; }
        public string? SummaryAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? PageSectionMedia { get; set; }
        public string? YoutubeUrl { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}