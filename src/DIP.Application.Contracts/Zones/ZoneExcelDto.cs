using System;

namespace DIP.Zones
{
    public class ZoneExcelDto
    {
        public string TitleEn { get; set; }
        public string TitleAR { get; set; }
        public string? MetaTitleEn { get; set; }
        public string? MetaDescriptionEn { get; set; }
        public string? MetaTitleAr { get; set; }
        public string? MetaDescriptionAr { get; set; }
        public string Slug { get; set; }
        public string? SummaryEn { get; set; }
        public string? SummaryAr { get; set; }
        public string? Image { get; set; }
        public string? HeaderImage { get; set; }
        public int Order { get; set; }
        public bool IsFeature { get; set; }
        public bool IsActive { get; set; }
    }
}