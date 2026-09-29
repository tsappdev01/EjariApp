using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;
using DIP.Validation;

namespace DIP.ContactForms
{
    public class ContactFormUpdateDto : IHasConcurrencyStamp
    {
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string FullName { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "EmailAddress")]
        public string Email { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}