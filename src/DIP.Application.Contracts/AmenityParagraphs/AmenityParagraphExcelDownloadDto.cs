using Volo.Abp.Application.Dtos;
using System;

namespace DIP.AmenityParagraphs
{
    public class AmenityParagraphExcelDownloadDto
    {
        public string DownloadToken { get; set; }

        public string? FilterText { get; set; }

        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string? SubTitleEn { get; set; }
        public string? SubTitleAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? ButtonUrl { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public bool? IsActive { get; set; }
        public Guid? AmenityId { get; set; }

        public AmenityParagraphExcelDownloadDto()
        {

        }
    }
}