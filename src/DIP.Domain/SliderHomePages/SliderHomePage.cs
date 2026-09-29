using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.SliderHomePages
{
    public class SliderHomePage : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string TitleAr { get; set; }

        [NotNull]
        public virtual string TitleEn { get; set; }

        [NotNull]
        public virtual string DescriptionAr { get; set; }

        [NotNull]
        public virtual string DescriptionEn { get; set; }

        [CanBeNull]
        public virtual string? ButtonTitleAr { get; set; }

        [CanBeNull]
        public virtual string? ButtonTitleEn { get; set; }

        [CanBeNull]
        public virtual string? ButtonUrlEn { get; set; }

        [CanBeNull]
        public virtual string? ButtonUrlAr { get; set; }

        [CanBeNull]
        public virtual string? Image { get; set; }

        [CanBeNull]
        public virtual string? YoutubeUrl { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual int Order { get; set; }

        public SliderHomePage()
        {

        }

        public SliderHomePage(Guid id, string titleAr, string titleEn, string descriptionAr, string descriptionEn, bool isActive, int order, string? buttonTitleAr = null, string? buttonTitleEn = null, string? buttonUrlEn = null, string? buttonUrlAr = null, string? image = null, string? youtubeUrl = null)
        {

            Id = id;
            Check.NotNull(titleAr, nameof(titleAr));
            Check.NotNull(titleEn, nameof(titleEn));
            Check.NotNull(descriptionAr, nameof(descriptionAr));
            Check.NotNull(descriptionEn, nameof(descriptionEn));
            TitleAr = titleAr;
            TitleEn = titleEn;
            DescriptionAr = descriptionAr;
            DescriptionEn = descriptionEn;
            IsActive = isActive;
            Order = order;
            ButtonTitleAr = buttonTitleAr;
            ButtonTitleEn = buttonTitleEn;
            ButtonUrlEn = buttonUrlEn;
            ButtonUrlAr = buttonUrlAr;
            Image = image;
            YoutubeUrl = youtubeUrl;
        }

    }
}