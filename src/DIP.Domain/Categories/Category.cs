using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.Categories
{
    public class Category : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string TitleAr { get; set; }

        [NotNull]
        public virtual string TitleEn { get; set; }

        public virtual int Order { get; set; }

        public virtual bool IsFeature { get; set; }

        public virtual bool IsActive { get; set; }

        public Category()
        {

        }

        public Category(Guid id, string titleAr, string titleEn, int order, bool isFeature, bool isActive)
        {

            Id = id;
            Check.NotNull(titleAr, nameof(titleAr));
            Check.NotNull(titleEn, nameof(titleEn));
            TitleAr = titleAr;
            TitleEn = titleEn;
            Order = order;
            IsFeature = isFeature;
            IsActive = isActive;
        }

    }
}