using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DIP.Validation;

namespace DIP.EServices
{
    public class OnlinePaymentDto
    {
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string TenantName { get; set; }

      
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        [MaxLength(9, ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "MaxLength")]
        [MinLength(9, ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "MaxLength")]
        public string MobileNumber { get; set; }


        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "PropertyCodeRequired")]
        public string PropertyCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "PropertyValueRequired")]
        [StringLength(4, MinimumLength = 1 , ErrorMessageResourceType =typeof(ValidationRes) , ErrorMessageResourceName = "PropertyCode")]
        public string PropertyValue { get; set; }


        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "EmailAddress")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        List<ServiceForPaymentDto> ServicesForPayment { get; set; }

 
    }

  public class  ServiceForPaymentDto
    {
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string Month { get; set; }


        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string Year { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string Amount { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string paymentFor { get; set; }


        public string Comments { get; set; }

      
    }
    public class  ServiceForPaymentCaptchaDto
    {
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string Captcha { get; set; }
    }

}