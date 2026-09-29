using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.LastEventss
{
    public class LastEvents : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string TitleAr { get; set; }

        [NotNull]
        public virtual string TitleEn { get; set; }

        [CanBeNull]
        public virtual string? MetaTitleAr { get; set; }

        [CanBeNull]
        public virtual string? MetaTitleEn { get; set; }

        [CanBeNull]
        public virtual string? MetaDescriptionEn { get; set; }

        [CanBeNull]
        public virtual string? MetaDescriptionAr { get; set; }

        public virtual bool IsFeatured { get; set; }

        [NotNull]
        public virtual string Slug { get; set; }

        [CanBeNull]
        public virtual string? Image { get; set; }

        [CanBeNull]
        public virtual string? HeaderImage { get; set; }

        [CanBeNull]
        public virtual string? DescriptionAr { get; set; }

        [CanBeNull]
        public virtual string? DescriptionEn { get; set; }

        [NotNull]
        public virtual string SummaryEn { get; set; }

        [NotNull]
        public virtual string SummaryAr { get; set; }

        public virtual int Order { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual DateTime EndDate { get; set; }

        public virtual bool IsActive { get; set; }

        [CanBeNull]
        public virtual string? LocationAr { get; set; }

        [CanBeNull]
        public virtual string? LocationEn { get; set; }

        public LastEvents()
        {

        }

        public LastEvents(Guid id, string titleAr, string titleEn, string metaTitleAr, string metaTitleEn, string metaDescriptionEn, string metaDescriptionAr, bool isFeatured, string slug, string image, string headerImage, string descriptionAr, string descriptionEn, string summaryEn, string summaryAr, int order, DateTime startDate, DateTime endDate, bool isActive, string locationAr, string locationEn)
        {

            Id = id;
            Check.NotNull(titleAr, nameof(titleAr));
            Check.NotNull(titleEn, nameof(titleEn));
            Check.NotNull(slug, nameof(slug));
            Check.NotNull(summaryEn, nameof(summaryEn));
            Check.NotNull(summaryAr, nameof(summaryAr));
            TitleAr = titleAr;
            TitleEn = titleEn;
            MetaTitleAr = metaTitleAr;
            MetaTitleEn = metaTitleEn;
            MetaDescriptionEn = metaDescriptionEn;
            MetaDescriptionAr = metaDescriptionAr;
            IsFeatured = isFeatured;
            Slug = slug;
            Image = image;
            HeaderImage = headerImage;
            DescriptionAr = descriptionAr;
            DescriptionEn = descriptionEn;
            SummaryEn = summaryEn;
            SummaryAr = summaryAr;
            Order = order;
            StartDate = startDate;
            EndDate = endDate;
            IsActive = isActive;
            LocationAr = locationAr;
            LocationEn = locationEn;
        }

    }
}