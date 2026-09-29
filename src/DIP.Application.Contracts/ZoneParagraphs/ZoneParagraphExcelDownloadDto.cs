using Volo.Abp.Application.Dtos;
using System;

namespace DIP.ZoneParagraphs
{
    public class ZoneParagraphExcelDownloadDto
    {
        public string DownloadToken { get; set; }

        public string? FilterText { get; set; }

        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string? SubTilteEn { get; set; }
        public string? SubTitleAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public bool? IsActive { get; set; }
        public Guid? ZoneId { get; set; }

        public ZoneParagraphExcelDownloadDto()
        {

        }
    }
}