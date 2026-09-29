using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using DIP.Validation;

namespace DIP.ContactForms
{
    public class ContactFormCreateDto
    {
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]


        public string FullName { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]

        [EmailAddress(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "EmailAddress")]
        public string Email { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }

        [Required]
        public string Captcha { get; set; }
    }
}