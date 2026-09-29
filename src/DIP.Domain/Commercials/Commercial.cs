using DIP.SubCategories;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.Commercials
{
    public class Commercial : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string TitleEn { get; set; }

        [NotNull]
        public virtual string TitleAr { get; set; }

        [CanBeNull]
        public virtual string? PlotNo { get; set; }

        [CanBeNull]
        public virtual string? ActivityEn { get; set; }

        [CanBeNull]
        public virtual string? ActivityAr { get; set; }

        [CanBeNull]
        public virtual string? Phone { get; set; }

        [CanBeNull]
        public virtual string? Fax { get; set; }

        [CanBeNull]
        public virtual string? MakaniNo { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual int Order { get; set; }
        public Guid SubCategoryId { get; set; }

        public Commercial()
        {

        }

        public Commercial(Guid id, Guid subCategoryId, string titleEn, string titleAr, string plotNo, string activityEn, string activityAr, string phone, string fax, string makaniNo, bool isActive, int order)
        {

            Id = id;
            Check.NotNull(titleEn, nameof(titleEn));
            Check.NotNull(titleAr, nameof(titleAr));
            TitleEn = titleEn;
            TitleAr = titleAr;
            PlotNo = plotNo;
            ActivityEn = activityEn;
            ActivityAr = activityAr;
            Phone = phone;
            Fax = fax;
            MakaniNo = makaniNo;
            IsActive = isActive;
            Order = order;
            SubCategoryId = subCategoryId;
        }

    }
}