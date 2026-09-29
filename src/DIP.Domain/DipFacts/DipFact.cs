using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.DipFacts
{
    public class DipFact : FullAuditedAggregateRoot<Guid>
    {
        [CanBeNull]
        public virtual string? Image { get; set; }

        [NotNull]
        public virtual string TitleAr { get; set; }

        [NotNull]
        public virtual string TitleEn { get; set; }

        [NotNull]
        public virtual string DescriptionAr { get; set; }

        [NotNull]
        public virtual string DescriptionEn { get; set; }

        public virtual int Order { get; set; }

        public virtual bool IsActive { get; set; }

        public DipFact()
        {

        }

        public DipFact(Guid id, string image, string titleAr, string titleEn, string descriptionAr, string descriptionEn, int order, bool isActive)
        {

            Id = id;
            Check.NotNull(titleAr, nameof(titleAr));
            Check.NotNull(titleEn, nameof(titleEn));
            Check.NotNull(descriptionAr, nameof(descriptionAr));
            Check.NotNull(descriptionEn, nameof(descriptionEn));
            Image = image;
            TitleAr = titleAr;
            TitleEn = titleEn;
            DescriptionAr = descriptionAr;
            DescriptionEn = descriptionEn;
            Order = order;
            IsActive = isActive;
        }

    }
}