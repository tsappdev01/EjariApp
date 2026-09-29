using DIP.PageInfos;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.PageInfoSections
{
    public class PageInfoSection : FullAuditedAggregateRoot<Guid>
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
        public virtual string? SummaryEn { get; set; }

        [CanBeNull]
        public virtual string? SummaryAr { get; set; }

        [CanBeNull]
        public virtual string? DescriptionEn { get; set; }

        [CanBeNull]
        public virtual string? DescriptionAr { get; set; }

        [CanBeNull]
        public virtual string? PageSectionMedia { get; set; }

        [CanBeNull]
        public virtual string? YoutubeUrl { get; set; }

        public virtual int Order { get; set; }

        public virtual bool IsActive { get; set; }
        public Guid PageInfoId { get; set; }

        public PageInfoSection()
        {

        }

        public PageInfoSection(Guid id, Guid pageInfoId, string titleEn, string titleAr, string subTitleEn, string subTitleAr, string summaryEn, string summaryAr, string descriptionEn, string descriptionAr, string pageSectionMedia, string youtubeUrl, int order, bool isActive)
        {

            Id = id;
            Check.NotNull(titleEn, nameof(titleEn));
            Check.NotNull(titleAr, nameof(titleAr));
            TitleEn = titleEn;
            TitleAr = titleAr;
            SubTitleEn = subTitleEn;
            SubTitleAr = subTitleAr;
            SummaryEn = summaryEn;
            SummaryAr = summaryAr;
            DescriptionEn = descriptionEn;
            DescriptionAr = descriptionAr;
            PageSectionMedia = pageSectionMedia;
            YoutubeUrl = youtubeUrl;
            Order = order;
            IsActive = isActive;
            PageInfoId = pageInfoId;
        }

    }
}