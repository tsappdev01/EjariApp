using Volo.Abp.Application.Dtos;
using System;

namespace DIP.Medias
{
    public class MediaExcelDownloadDto
    {
        public string DownloadToken { get; set; }

        public string? FilterText { get; set; }

        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string? File { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public bool? IsActive { get; set; }
        public Guid? ZoneParagraphId { get; set; }
        public Guid? AmenityParagraphId { get; set; }
        public Guid? MediaGalleryId { get; set; }

        public MediaExcelDownloadDto()
        {

        }
    }
}