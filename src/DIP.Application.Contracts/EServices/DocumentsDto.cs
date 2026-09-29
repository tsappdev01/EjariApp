using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using DIP.Localization;
using DIP.Validation;

namespace DIP.EServices
{
    public class DocumentDto
    {



        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string ReferenceNumber { get; set; }


        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]


        public string DocumentId { get; set; }

        [Required]
        public string Captcha { get; set; }


    }
}