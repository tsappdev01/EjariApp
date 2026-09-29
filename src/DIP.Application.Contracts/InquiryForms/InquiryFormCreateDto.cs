using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using DIP.Validation;

namespace DIP.InquiryForms
{
    public class InquiryFormCreateDto
    {
        public string? CompanyName { get; set; }


        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]

        [EmailAddress(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "EmailAddress")]
        public string Email { get; set; }


        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string Mobile { get; set; }
        public string? Fax { get; set; }
        public string? Phone { get; set; }
        public string? InquiryType { get; set; }
        public string? TradeLicensePlateOfIssue { get; set; }
        public string? BuyRent { get; set; }
        public double SpaceInSquareFeet { get; set; }
        public string? Comments { get; set; }

        [Required]
        public string Captcha { get; set; }
    }
}