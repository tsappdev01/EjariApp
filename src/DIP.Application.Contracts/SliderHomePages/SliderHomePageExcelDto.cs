using System;

namespace DIP.SliderHomePages
{
    public class SliderHomePageExcelDto
    {
        public string TitleAr { get; set; } = null!;
        public string TitleEn { get; set; } = null!;
        public string DescriptionAr { get; set; } = null!;
        public string DescriptionEn { get; set; } = null!;
        public string? ButtonTitleAr { get; set; }
        public string? ButtonTitleEn { get; set; }
        public string? ButtonUrlEn { get; set; }
        public string? ButtonUrlAr { get; set; }
        public string? Image { get; set; }
        public string? YoutubeUrl { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
    }
}