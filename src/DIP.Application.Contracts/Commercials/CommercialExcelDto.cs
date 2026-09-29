using System;

namespace DIP.Commercials
{
    public class CommercialExcelDto
    {
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string? PlotNo { get; set; }
        public string? ActivityEn { get; set; }
        public string? ActivityAr { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public string? MakaniNo { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
    }
}