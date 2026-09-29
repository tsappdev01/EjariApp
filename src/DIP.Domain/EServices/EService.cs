using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.EServices
{
    public class EService : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string TitleEn { get; set; }

        [CanBeNull]
        public virtual string? TitleAr { get; set; }

        [NotNull]
        public virtual string Slug { get; set; }

        [CanBeNull]
        public virtual string? DescriptionEn { get; set; }

        [CanBeNull]
        public virtual string? DescriptionAr { get; set; }

        [CanBeNull]
        public virtual string? Image { get; set; }

        [CanBeNull]
        public virtual string? HeaderImage { get; set; }

        [CanBeNull]
        public virtual string? MetaTitleEn { get; set; }

        [CanBeNull]
        public virtual string? MetaTitleAr { get; set; }

        [CanBeNull]
        public virtual string? MetaDescriptionEn { get; set; }

        [CanBeNull]
        public virtual string? MetaDescriptionAr { get; set; }

        public virtual int Order { get; set; }

        public virtual bool IsActive { get; set; }

        public EService()
        {

        }

        public EService(Guid id, string titleEn, string titleAr, string slug, string descriptionEn, string descriptionAr, string image, string headerImage, string metaTitleEn, string metaTitleAr, string metaDescriptionEn, string metaDescriptionAr, int order, bool isActive)
        {

            Id = id;
            Check.NotNull(titleEn, nameof(titleEn));
            Check.NotNull(slug, nameof(slug));
            TitleEn = titleEn;
            TitleAr = titleAr;
            Slug = slug;
            DescriptionEn = descriptionEn;
            DescriptionAr = descriptionAr;
            Image = image;
            HeaderImage = headerImage;
            MetaTitleEn = metaTitleEn;
            MetaTitleAr = metaTitleAr;
            MetaDescriptionEn = metaDescriptionEn;
            MetaDescriptionAr = metaDescriptionAr;
            Order = order;
            IsActive = isActive;
        }

    }
}