using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.EFormServices
{
    public class EFormService : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string TitleEn { get; set; }

        [CanBeNull]
        public virtual string? TitleAr { get; set; }

        public virtual int Order { get; set; }

        public virtual bool IsActive { get; set; }

        public EFormService()
        {

        }

        public EFormService(Guid id, string titleEn, string titleAr, int order, bool isActive)
        {

            Id = id;
            Check.NotNull(titleEn, nameof(titleEn));
            TitleEn = titleEn;
            TitleAr = titleAr;
            Order = order;
            IsActive = isActive;
        }

    }
}