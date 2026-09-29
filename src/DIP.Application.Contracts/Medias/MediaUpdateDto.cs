using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace DIP.Medias
{
    public class MediaUpdateDto : IHasConcurrencyStamp
    {
        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string? File { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public Guid? ZoneParagraphId { get; set; }
        public Guid? AmenityParagraphId { get; set; }
        public Guid? MediaGalleryId { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}