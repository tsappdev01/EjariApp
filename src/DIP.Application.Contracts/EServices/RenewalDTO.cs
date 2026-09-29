using DIP.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIP.EServices
{
    public class RenewalDTO
    {
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string EjariContractNo { get; set; }


        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessageResourceType = typeof(ValidationRes),
        ErrorMessageResourceName = "EmailAddress")]
        public string EmailAddress { get; set; }

        [Required]
        public string Captcha { get; set; }
    }
}
