using System;

namespace DIP.TimeLines
{
    public class TimeLineExcelDto
    {
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? Image { get; set; }
        public DateTime? TimeLineDate { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}