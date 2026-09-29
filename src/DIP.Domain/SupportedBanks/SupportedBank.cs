using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.SupportedBanks
{
    public class SupportedBank : FullAuditedAggregateRoot<Guid>
    {
        [CanBeNull]
        public virtual string? TitleAr { get; set; }

        [CanBeNull]
        public virtual string? TitleEn { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual int Order { get; set; }

        public SupportedBank()
        {

        }

        public SupportedBank(Guid id, string titleAr, string titleEn, bool isActive, int order)
        {

            Id = id;
            TitleAr = titleAr;
            TitleEn = titleEn;
            IsActive = isActive;
            Order = order;
        }

    }
}