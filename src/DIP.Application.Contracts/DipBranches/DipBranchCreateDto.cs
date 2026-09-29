using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DIP.DipBranches
{
    public class DipBranchCreateDto
    {
        [Required]
        public string TitleEn { get; set; }
        [Required]
        public string TitleAr { get; set; }
        public string? SubTitleEn { get; set; }
        public string? SubTitleAr { get; set; }
        public string? Phone { get; set; }
        public string? AlternativePhone { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [EmailAddress]
        public string? AlternativeEmail { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;
    }
}