using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.Amenities
{
    public class Amenity : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string TitleEn { get; set; }

        [NotNull]
        public virtual string TitleAr { get; set; }

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
        public virtual string? HeaderImage { get; set; }

        [CanBeNull]
        public virtual string? Image { get; set; }

        public virtual int Order { get; set; }

        public virtual bool IsActive { get; set; }

        public Amenity()
        {

        }

        public Amenity(Guid id, string titleEn, string titleAr, string metaTitleEn, string metaDescriptionEn, string metaTitleAr, string metaDescriptionAr, string slug, string headerImage, string image, int order, bool isActive)
        {

            Id = id;
            Check.NotNull(titleEn, nameof(titleEn));
            Check.NotNull(titleAr, nameof(titleAr));
            Check.NotNull(slug, nameof(slug));
            TitleEn = titleEn;
            TitleAr = titleAr;
            MetaTitleEn = metaTitleEn;
            MetaDescriptionEn = metaDescriptionEn;
            MetaTitleAr = metaTitleAr;
            MetaDescriptionAr = metaDescriptionAr;
            Slug = slug;
            HeaderImage = headerImage;
            Image = image;
            Order = order;
            IsActive = isActive;
        }

    }
}