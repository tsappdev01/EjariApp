using DIP.Zones;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.ZoneParagraphs
{
    public class ZoneParagraph : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string TitleEn { get; set; }

        [NotNull]
        public virtual string TitleAr { get; set; }

        [CanBeNull]
        public virtual string? SubTilteEn { get; set; }

        [CanBeNull]
        public virtual string? SubTitleAr { get; set; }

        [CanBeNull]
        public virtual string? DescriptionEn { get; set; }

        [CanBeNull]
        public virtual string? DescriptionAr { get; set; }

        public virtual int Order { get; set; }

        public virtual bool IsActive { get; set; }
        public Guid ZoneId { get; set; }

        public ZoneParagraph()
        {

        }

        public ZoneParagraph(Guid id, Guid zoneId, string titleEn, string titleAr, string subTilteEn, string subTitleAr, string descriptionEn, string descriptionAr, int order, bool isActive)
        {

            Id = id;
            Check.NotNull(titleEn, nameof(titleEn));
            Check.NotNull(titleAr, nameof(titleAr));
            TitleEn = titleEn;
            TitleAr = titleAr;
            SubTilteEn = subTilteEn;
            SubTitleAr = subTitleAr;
            DescriptionEn = descriptionEn;
            DescriptionAr = descriptionAr;
            Order = order;
            IsActive = isActive;
            ZoneId = zoneId;
        }

    }
}