using System;

namespace DIP.Amenities
{
    public class AmenityExcelDto
    {
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string? MetaTitleEn { get; set; }
        public string? MetaDescriptionEn { get; set; }
        public string? MetaTitleAr { get; set; }
        public string? MetaDescriptionAr { get; set; }
        public string Slug { get; set; }
        public string? HeaderImage { get; set; }
        public string? Image { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}