using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.InquiryForms
{
    public class InquiryForm : FullAuditedAggregateRoot<Guid>
    {
        [CanBeNull]
        public virtual string? CompanyName { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        [CanBeNull]
        public virtual string? Email { get; set; }

        [NotNull]
        public virtual string Mobile { get; set; }

        [CanBeNull]
        public virtual string? Fax { get; set; }

        [CanBeNull]
        public virtual string? Phone { get; set; }

        [CanBeNull]
        public virtual string? InquiryType { get; set; }

        [CanBeNull]
        public virtual string? TradeLicensePlateOfIssue { get; set; }

        [CanBeNull]
        public virtual string? BuyRent { get; set; }

        public virtual double SpaceInSquareFeet { get; set; }

        [CanBeNull]
        public virtual string? Comments { get; set; }

        public InquiryForm()
        {

        }

        public InquiryForm(Guid id, string companyName, string name, string email, string mobile, string fax, string phone, string inquiryType, string tradeLicensePlateOfIssue, string buyRent, double spaceInSquareFeet, string comments)
        {

            Id = id;
            Check.NotNull(name, nameof(name));
            Check.NotNull(mobile, nameof(mobile));
            CompanyName = companyName;
            Name = name;
            Email = email;
            Mobile = mobile;
            Fax = fax;
            Phone = phone;
            InquiryType = inquiryType;
            TradeLicensePlateOfIssue = tradeLicensePlateOfIssue;
            BuyRent = buyRent;
            SpaceInSquareFeet = spaceInSquareFeet;
            Comments = comments;
        }

    }
}