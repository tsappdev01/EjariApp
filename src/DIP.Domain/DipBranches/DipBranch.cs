using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.DipBranches
{
    public class DipBranch : FullAuditedAggregateRoot<Guid>
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
        public virtual string? Phone { get; set; }

        [CanBeNull]
        public virtual string? AlternativePhone { get; set; }

        [CanBeNull]
        public virtual string? Email { get; set; }

        [CanBeNull]
        public virtual string? AlternativeEmail { get; set; }

        public virtual int Order { get; set; }

        public virtual bool IsActive { get; set; }

        public DipBranch()
        {

        }

        public DipBranch(Guid id, string titleEn, string titleAr, string subTitleEn, string subTitleAr, string phone, string alternativePhone, string email, string alternativeEmail, int order, bool isActive)
        {

            Id = id;
            Check.NotNull(titleEn, nameof(titleEn));
            Check.NotNull(titleAr, nameof(titleAr));
            TitleEn = titleEn;
            TitleAr = titleAr;
            SubTitleEn = subTitleEn;
            SubTitleAr = subTitleAr;
            Phone = phone;
            AlternativePhone = alternativePhone;
            Email = email;
            AlternativeEmail = alternativeEmail;
            Order = order;
            IsActive = isActive;
        }

    }
}