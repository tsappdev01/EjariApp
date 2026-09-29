using System;

namespace DIP.SubCategories
{
    public class SubCategoryExcelDto
    {
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public int Order { get; set; }
        public bool IsFeature { get; set; }
        public bool IsActive { get; set; }
    }
}