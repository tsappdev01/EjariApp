using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using DIP.Validation;

namespace DIP.FeedBacks
{
    public class FeedBackCreateDto
    {
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string Subject { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string CompanyName { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string PlotNo { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string PlotCategory { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string ContactPersonName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "EmailAddress")]
        [DataType(DataType.EmailAddress)]
        public string EmailId { get; set; }


        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string MobileNumber { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string Department { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string CategoryName { get; set; }
         [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string Description { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string Captcha { get; set; }
    }
}