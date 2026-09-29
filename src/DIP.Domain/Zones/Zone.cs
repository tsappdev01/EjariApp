using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.Zones
{
    public class Zone : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string TitleEn { get; set; }

        [NotNull]
        public virtual string TitleAR { get; set; }

        [CanBeNull]
        public virtual string? MetaTitleEn { get; set; }

        [CanBeNull]
        public virtual string? MetaDescriptionEn { get; set; }

        [CanBeNull]
        public virtual string? MetaTitleAr { get; set; }

        [CanBeNull]
        public virtual string? MetaDescriptionAr { get; set; }

        [NotNull]
        public virtual string Slug { get; set; }

        [CanBeNull]
        public virtual string? SummaryEn { get; set; }

        [CanBeNull]
        public virtual string? SummaryAr { get; set; }

        [CanBeNull]
        public virtual string? Image { get; set; }

        [CanBeNull]
        public virtual string? HeaderImage { get; set; }

        public virtual int Order { get; set; }

        public virtual bool IsFeature { get; set; }

        public virtual bool IsActive { get; set; }

        public Zone()
        {

        }

        public Zone(Guid id, string titleEn, string titleAR, string metaTitleEn, string metaDescriptionEn, string metaTitleAr, string metaDescriptionAr, string slug, string summaryEn, string summaryAr, string image, string headerImage, int order, bool isFeature, bool isActive)
        {

            Id = id;
            Check.NotNull(titleEn, nameof(titleEn));
            Check.NotNull(titleAR, nameof(titleAR));
            Check.NotNull(slug, nameof(slug));
            TitleEn = titleEn;
            TitleAR = titleAR;
            MetaTitleEn = metaTitleEn;
            MetaDescriptionEn = metaDescriptionEn;
            MetaTitleAr = metaTitleAr;
            MetaDescriptionAr = metaDescriptionAr;
            Slug = slug;
            SummaryEn = summaryEn;
            SummaryAr = summaryAr;
            Image = image;
            HeaderImage = headerImage;
            Order = order;
            IsFeature = isFeature;
            IsActive = isActive;
        }

    }
}