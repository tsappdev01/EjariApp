using System;

namespace DIP.PressReleases
{
    public class PressReleaseExcelDto
    {
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string? MetaTitleAr { get; set; }
        public string? MetaTitleEn { get; set; }
        public string? MetaDescriptionEn { get; set; }
        public string? MetaDescriptionAr { get; set; }
        public bool IsFeatured { get; set; }
        public string Slug { get; set; }
        public string? Image { get; set; }
        public string? HeaderImage { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string SummaryEn { get; set; }
        public string SummaryAr { get; set; }
        public int Order { get; set; }
        public DateTime Date { get; set; }
        public bool IsActive { get; set; }
    }
}