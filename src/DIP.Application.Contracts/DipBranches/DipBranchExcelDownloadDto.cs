using Volo.Abp.Application.Dtos;
using System;

namespace DIP.DipBranches
{
    public class DipBranchExcelDownloadDto
    {
        public string DownloadToken { get; set; }

        public string? FilterText { get; set; }

        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string? SubTitleEn { get; set; }
        public string? SubTitleAr { get; set; }
        public string? Phone { get; set; }
        public string? AlternativePhone { get; set; }
        public string? Email { get; set; }
        public string? AlternativeEmail { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public bool? IsActive { get; set; }

        public DipBranchExcelDownloadDto()
        {

        }
    }
}