using DIP.ZoneParagraphs;
using DIP.AmenityParagraphs;
using DIP.MediaGalleries;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.Medias
{
    public class Media : FullAuditedAggregateRoot<Guid>
    {
        [CanBeNull]
        public virtual string? TitleEn { get; set; }

        [CanBeNull]
        public virtual string? TitleAr { get; set; }

        [CanBeNull]
        public virtual string? File { get; set; }

        public virtual int Order { get; set; }

        public virtual bool IsActive { get; set; }
        public Guid? ZoneParagraphId { get; set; }
        public Guid? AmenityParagraphId { get; set; }
        public Guid? MediaGalleryId { get; set; }

        public Media()
        {

        }

        public Media(Guid id, Guid? zoneParagraphId, Guid? amenityParagraphId, Guid? mediaGalleryId, string titleEn, string titleAr, string file, int order, bool isActive)
        {

            Id = id;
            TitleEn = titleEn;
            TitleAr = titleAr;
            File = file;
            Order = order;
            IsActive = isActive;
            ZoneParagraphId = zoneParagraphId;
            AmenityParagraphId = amenityParagraphId;
            MediaGalleryId = mediaGalleryId;
        }

    }
}