using DIP.Amenities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.AmenityParagraphs
{
    public class AmenityParagraph : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string TitleEn { get; set; }

        [NotNull]
        public virtual string TitleAr { get; set; }

        [CanBeNull]
        public virtual string? SubTitleEn { get; set; }

        [CanBeNull]
        public virtual string? SubTitleAr { get; set; }

        [CanBeNull]
        public virtual string? DescriptionEn { get; set; }

        [CanBeNull]
        public virtual string? DescriptionAr { get; set; }

        [CanBeNull]
        public virtual string? ButtonUrl { get; set; }

        public virtual int Order { get; set; }

        public virtual bool IsActive { get; set; }
        public Guid AmenityId { get; set; }

        public AmenityParagraph()
        {

        }

        public AmenityParagraph(Guid id, Guid amenityId, string titleEn, string titleAr, string subTitleEn, string subTitleAr, string descriptionEn, string descriptionAr, string buttonUrl, int order, bool isActive)
        {

            Id = id;
            Check.NotNull(titleEn, nameof(titleEn));
            Check.NotNull(titleAr, nameof(titleAr));
            TitleEn = titleEn;
            TitleAr = titleAr;
            SubTitleEn = subTitleEn;
            SubTitleAr = subTitleAr;
            DescriptionEn = descriptionEn;
            DescriptionAr = descriptionAr;
            ButtonUrl = buttonUrl;
            Order = order;
            IsActive = isActive;
            AmenityId = amenityId;
        }

    }
}