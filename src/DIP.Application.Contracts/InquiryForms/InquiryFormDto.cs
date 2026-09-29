using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.InquiryForms
{
    public class InquiryFormDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string? CompanyName { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string Mobile { get; set; }
        public string? Fax { get; set; }
        public string? Phone { get; set; }
        public string? InquiryType { get; set; }
        public string? TradeLicensePlateOfIssue { get; set; }
        public string? BuyRent { get; set; }
        public double SpaceInSquareFeet { get; set; }
        public string? Comments { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}