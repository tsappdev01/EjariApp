using Volo.Abp.Application.Dtos;
using System;

namespace DIP.DipFacts
{
    public class DipFactExcelDownloadDto
    {
        public string DownloadToken { get; set; }

        public string? FilterText { get; set; }

        public string? Image { get; set; }
        public string? TitleAr { get; set; }
        public string? TitleEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public bool? IsActive { get; set; }

        public DipFactExcelDownloadDto()
        {

        }
    }
}