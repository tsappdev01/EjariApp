using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.MediaGalleries
{
    public class MediaGallery : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string TitleEn { get; set; }

        [NotNull]
        public virtual string Slug { get; set; }

        [CanBeNull]
        public virtual string? TitleAr { get; set; }

        [CanBeNull]
        public virtual string? MetaTitleEn { get; set; }

        [CanBeNull]
        public virtual string? MetaTitleAr { get; set; }

        [CanBeNull]
        public virtual string? MetaDescriptionEn { get; set; }

        [CanBeNull]
        public virtual string? MetaDescriptionAr { get; set; }

        [CanBeNull]
        public virtual string? SummaryEn { get; set; }

        [CanBeNull]
        public virtual string? SummaryAr { get; set; }

        [CanBeNull]
        public virtual string? HeaderImage { get; set; }

        [CanBeNull]
        public virtual string? Image { get; set; }

        public virtual int Order { get; set; }

        public virtual bool IsActive { get; set; }

        public MediaGallery()
        {

        }

        public MediaGallery(Guid id, string titleEn, string slug, string titleAr, string metaTitleEn, string metaTitleAr, string metaDescriptionEn, string metaDescriptionAr, string summaryEn, string summaryAr, string headerImage, string image, int order, bool isActive)
        {

            Id = id;
            Check.NotNull(titleEn, nameof(titleEn));
            Check.NotNull(slug, nameof(slug));
            TitleEn = titleEn;
            Slug = slug;
            TitleAr = titleAr;
            MetaTitleEn = metaTitleEn;
            MetaTitleAr = metaTitleAr;
            MetaDescriptionEn = metaDescriptionEn;
            MetaDescriptionAr = metaDescriptionAr;
            SummaryEn = summaryEn;
            SummaryAr = summaryAr;
            HeaderImage = headerImage;
            Image = image;
            Order = order;
            IsActive = isActive;
        }

    }
}