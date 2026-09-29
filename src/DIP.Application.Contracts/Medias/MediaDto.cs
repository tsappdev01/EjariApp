using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.Medias
{
    public class MediaDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }

        public string? OldFileName { get; set; }
        public string? File { get; set; }
        public byte[] FileContent { get; set; }
        public bool FileNewUpload { get; set; } = false;
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public Guid? ZoneParagraphId { get; set; }
        public Guid? AmenityParagraphId { get; set; }
        public Guid? MediaGalleryId { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}