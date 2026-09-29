using DIP.TimeLineCategories;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.TimeLines
{
    public class TimeLine : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string TitleEn { get; set; }

        [NotNull]
        public virtual string TitleAr { get; set; }

        [CanBeNull]
        public virtual string? DescriptionEn { get; set; }

        [CanBeNull]
        public virtual string? DescriptionAr { get; set; }

        [CanBeNull]
        public virtual string? Image { get; set; }

        public virtual DateTime? TimeLineDate { get; set; }

        public virtual int Order { get; set; }

        public virtual bool IsActive { get; set; }
        public Guid TimeLineCategoryId { get; set; }

        public TimeLine()
        {

        }

        public TimeLine(Guid id, Guid timeLineCategoryId, string titleEn, string titleAr, string descriptionEn, string descriptionAr, string image, int order, bool isActive, DateTime? timeLineDate = null)
        {

            Id = id;
            Check.NotNull(titleEn, nameof(titleEn));
            Check.NotNull(titleAr, nameof(titleAr));
            TitleEn = titleEn;
            TitleAr = titleAr;
            DescriptionEn = descriptionEn;
            DescriptionAr = descriptionAr;
            Image = image;
            Order = order;
            IsActive = isActive;
            TimeLineDate = timeLineDate;
            TimeLineCategoryId = timeLineCategoryId;
        }

    }
}