using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace DIP.InquiryForms
{
    public class InquiryFormUpdateDto : IHasConcurrencyStamp
    {
        public string? CompanyName { get; set; }
        [Required]
        public string Name { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
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