using DIP.EFormServices;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.EFormServiceSubCategories
{
    public class EFormServiceSubCategory : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string TitleEn { get; set; }

        [CanBeNull]
        public virtual string? TitleAr { get; set; }

        [CanBeNull]
        public virtual string? File { get; set; }

        public virtual int Order { get; set; }

        public virtual bool IsActive { get; set; }
        public Guid EFormServiceId { get; set; }

        public EFormServiceSubCategory()
        {

        }

        public EFormServiceSubCategory(Guid id, Guid eFormServiceId, string titleEn, string titleAr, string file, int order, bool isActive)
        {

            Id = id;
            Check.NotNull(titleEn, nameof(titleEn));
            TitleEn = titleEn;
            TitleAr = titleAr;
            File = file;
            Order = order;
            IsActive = isActive;
            EFormServiceId = eFormServiceId;
        }

    }
}