using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.SiteSettings
{
    public class SiteSetting : FullAuditedAggregateRoot<Guid>
    {
        [CanBeNull]
        public virtual string? FaceBookLink { get; set; }

        [CanBeNull]
        public virtual string? TwitterLink { get; set; }

        [CanBeNull]
        public virtual string? InstagramLink { get; set; }

        [CanBeNull]
        public virtual string? YouTubeLink { get; set; }

        [CanBeNull]
        public virtual string? LinkedinLink { get; set; }

        [CanBeNull]
        public virtual string? FireDepartmentPhone { get; set; }

        [CanBeNull]
        public virtual string? Emergency1Phone { get; set; }

        [CanBeNull]
        public virtual string? Emergency2Phone { get; set; }

        [CanBeNull]
        public virtual string? PolicePost { get; set; }

        [CanBeNull]
        public virtual string? DipEmail { get; set; }

        [CanBeNull]
        public virtual string? Phone { get; set; }

        [CanBeNull]
        public virtual string? POBox { get; set; }

        [CanBeNull]
        public virtual string? OfficeLocationEn { get; set; }

        [CanBeNull]
        public virtual string? OfficeLocationAr { get; set; }

        [CanBeNull]
        public virtual string? SiteLink { get; set; }

        [CanBeNull]
        public virtual string? PbLocation { get; set; }

        [CanBeNull]
        public virtual string? WorkDays { get; set; }

        [CanBeNull]
        public virtual string? WorkHours { get; set; }

        [CanBeNull]
        public virtual string? RamadanWorkDays { get; set; }

        [CanBeNull]
        public virtual string? RamadanWorkHours { get; set; }

        [CanBeNull]
        public virtual string? FridayWorkHours { get; set; }

        [CanBeNull]
        public virtual string? ClosedDay1 { get; set; }

        [CanBeNull]
        public virtual string? ClosedDay2 { get; set; }

        public SiteSetting()
        {

        }

        public SiteSetting(Guid id, string faceBookLink, string twitterLink, string instagramLink, string youTubeLink, string linkedinLink, string fireDepartmentPhone, string emergency1Phone, string emergency2Phone, string policePost, string dipEmail, string phone, string pOBox, string officeLocationEn, string officeLocationAr, string siteLink, string pbLocation, string workDays, string workHours, string ramadanWorkDays, string ramadanWorkHours, string fridayWorkHours, string closedDay1, string closedDay2)
        {

            Id = id;
            FaceBookLink = faceBookLink;
            TwitterLink = twitterLink;
            InstagramLink = instagramLink;
            YouTubeLink = youTubeLink;
            LinkedinLink = linkedinLink;
            FireDepartmentPhone = fireDepartmentPhone;
            Emergency1Phone = emergency1Phone;
            Emergency2Phone = emergency2Phone;
            PolicePost = policePost;
            DipEmail = dipEmail;
            Phone = phone;
            POBox = pOBox;
            OfficeLocationEn = officeLocationEn;
            OfficeLocationAr = officeLocationAr;
            SiteLink = siteLink;
            PbLocation = pbLocation;
            WorkDays = workDays;
            WorkHours = workHours;
            RamadanWorkDays = ramadanWorkDays;
            RamadanWorkHours = ramadanWorkHours;
            FridayWorkHours = fridayWorkHours;
            ClosedDay1 = closedDay1;
            ClosedDay2 = closedDay2;
        }

    }
}