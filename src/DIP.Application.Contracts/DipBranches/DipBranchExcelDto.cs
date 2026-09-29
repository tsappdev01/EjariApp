using System;

namespace DIP.DipBranches
{
    public class DipBranchExcelDto
    {
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string? SubTitleEn { get; set; }
        public string? SubTitleAr { get; set; }
        public string? Phone { get; set; }
        public string? AlternativePhone { get; set; }
        public string? Email { get; set; }
        public string? AlternativeEmail { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}