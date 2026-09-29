using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using DIP.Validation;

namespace DIP.EServices
{
    public class LoginDto
    {

        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string ReferenceNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string PassCode { get; set; }

        [Required]
        public string Captcha { get; set; }



    }
}