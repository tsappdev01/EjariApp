using System;

namespace DIP.EFormServiceSubCategories
{
    public class EFormServiceSubCategoryExcelDto
    {
        public string TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string? File { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}