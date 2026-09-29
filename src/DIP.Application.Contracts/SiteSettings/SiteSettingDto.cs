using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.SiteSettings
{
    public class SiteSettingDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string? FaceBookLink { get; set; }
        public string? TwitterLink { get; set; }
        public string? InstagramLink { get; set; }
        public string? YouTubeLink { get; set; }
        public string? LinkedinLink { get; set; }
        public string? FireDepartmentPhone { get; set; }
        public string? Emergency1Phone { get; set; }
        public string? Emergency2Phone { get; set; }
        public string? PolicePost { get; set; }
        public string? DipEmail { get; set; }
        public string? Phone { get; set; }
        public string? POBox { get; set; }
        public string? OfficeLocationEn { get; set; }
        public string? OfficeLocationAr { get; set; }
        public string? SiteLink { get; set; }
        public string? PbLocation { get; set; }
        public string? WorkDays { get; set; }
        public string? WorkHours { get; set; }
        public string? RamadanWorkDays { get; set; }
        public string? RamadanWorkHours { get; set; }
        public string? FridayWorkHours { get; set; }
        public string? ClosedDay1 { get; set; }
        public string? ClosedDay2 { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}