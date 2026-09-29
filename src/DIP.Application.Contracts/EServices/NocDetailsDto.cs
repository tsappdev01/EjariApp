using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using DIP.Validation;
using DIP.Attributes;

namespace DIP.EServices
{
    public class NocDetailsDto
    {



        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public DateTime ContractFromDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        [DateGreaterThan("ContractFromDate", ErrorMessage = "Contract End Date must be greater than Contract From Date")]
        public DateTime ContractEndDate { get; set; }


        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public int NocType { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "ContractType must be greater than zero.")]
        public int ContractType { get; set; }

        public string ReferenceNumber { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "This field must be greater than zero.")]
        public double BasicRent { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        [Range(0, int.MaxValue, ErrorMessage = "Security Deposit must be zero or greater than zero.")]
        //[SecurityDepositConditionalRange(ErrorMessage = "Security Deposit must be greater than zero.")]
        public double SecurityDeposit { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public int IsSisterCompany { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        //[Range(1, int.MaxValue, ErrorMessageResourceType = typeof(ValidationRes))]
        public int NumberOfUnits { get; set; } = 0;


        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string NOCFor { get; set; }
        public string UnitsName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string BuildingName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        //public string Captcha { get; set; }


    }
}