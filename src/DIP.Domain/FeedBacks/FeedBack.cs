using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.FeedBacks
{
    public class FeedBack : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Subject { get; set; }

        [NotNull]
        public virtual string CompanyName { get; set; }

        [NotNull]
        public virtual string PlotNo { get; set; }

        [NotNull]
        public virtual string PlotCategory { get; set; }

        [NotNull]
        public virtual string ContactPersonName { get; set; }

        [NotNull]
        public virtual string EmailId { get; set; }

        [NotNull]
        public virtual string MobileNumber { get; set; }

        [NotNull]
        public virtual string Department { get; set; }

        [NotNull]
        public virtual string CategoryName { get; set; }

        [NotNull]
        public virtual string Description { get; set; }

        public FeedBack()
        {

        }

        public FeedBack(Guid id, string subject, string companyName, string plotNo, string plotCategory, string contactPersonName, string emailId, string mobileNumber, string department, string categoryName, string description)
        {

            Id = id;
            Check.NotNull(subject, nameof(subject));
            Check.NotNull(companyName, nameof(companyName));
            Check.NotNull(plotNo, nameof(plotNo));
            Check.NotNull(plotCategory, nameof(plotCategory));
            Check.NotNull(contactPersonName, nameof(contactPersonName));
            Check.NotNull(emailId, nameof(emailId));
            Check.NotNull(mobileNumber, nameof(mobileNumber));
            Check.NotNull(department, nameof(department));
            Check.NotNull(categoryName, nameof(categoryName));
            Check.NotNull(description, nameof(description));
            Subject = subject;
            CompanyName = companyName;
            PlotNo = plotNo;
            PlotCategory = plotCategory;
            ContactPersonName = contactPersonName;
            EmailId = emailId;
            MobileNumber = mobileNumber;
            Department = department;
            CategoryName = categoryName;
            Description = description;
        }

    }
}