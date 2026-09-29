using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.PageInfos
{
    public class PageInfo : FullAuditedAggregateRoot<Guid>
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

        [NotNull]
        public virtual string Slug { get; set; }

        [CanBeNull]
        public virtual string? Image { get; set; }

        [CanBeNull]
        public virtual string? HeaderImage { get; set; }

        [CanBeNull]
        public virtual string? YouTubeUrl { get; set; }

        [CanBeNull]
        public virtual string? PageInfoArticleTilteEn { get; set; }

        [CanBeNull]
        public virtual string? PageInfoArticleTilteAr { get; set; }

        [CanBeNull]
        public virtual string? PageInfoArticleSubtitleEn { get; set; }

        [CanBeNull]
        public virtual string? PageInfoArticleSubtitleAr { get; set; }

        [CanBeNull]
        public virtual string? DescriptionAr { get; set; }

        [CanBeNull]
        public virtual string? DescriptionEn { get; set; }

        [CanBeNull]
        public virtual string? SummaryEn { get; set; }

        [CanBeNull]
        public virtual string? SummaryAr { get; set; }

        public virtual int Order { get; set; }

        public virtual bool IsActive { get; set; }

        public PageInfo()
        {

        }

        public PageInfo(Guid id, string titleAr, string titleEn, string metaTitleAr, string metaTitleEn, string metaDescriptionEn, string metaDescriptionAr, string slug, string image, string headerImage, string youTubeUrl, string pageInfoArticleTilteEn, string pageInfoArticleTilteAr, string pageInfoArticleSubtitleEn, string pageInfoArticleSubtitleAr, string descriptionAr, string descriptionEn, string summaryEn, string summaryAr, int order, bool isActive)
        {

            Id = id;
            Check.NotNull(titleAr, nameof(titleAr));
            Check.NotNull(titleEn, nameof(titleEn));
            Check.NotNull(slug, nameof(slug));
            TitleAr = titleAr;
            TitleEn = titleEn;
            MetaTitleAr = metaTitleAr;
            MetaTitleEn = metaTitleEn;
            MetaDescriptionEn = metaDescriptionEn;
            MetaDescriptionAr = metaDescriptionAr;
            Slug = slug;
            Image = image;
            HeaderImage = headerImage;
            YouTubeUrl = youTubeUrl;
            PageInfoArticleTilteEn = pageInfoArticleTilteEn;
            PageInfoArticleTilteAr = pageInfoArticleTilteAr;
            PageInfoArticleSubtitleEn = pageInfoArticleSubtitleEn;
            PageInfoArticleSubtitleAr = pageInfoArticleSubtitleAr;
            DescriptionAr = descriptionAr;
            DescriptionEn = descriptionEn;
            SummaryEn = summaryEn;
            SummaryAr = summaryAr;
            Order = order;
            IsActive = isActive;
        }

    }
}