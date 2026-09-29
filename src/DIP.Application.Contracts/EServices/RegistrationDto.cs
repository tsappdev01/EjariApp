using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DIP.Validation;

namespace DIP.EServices
{
    public class RegistrationDto : IValidatableObject
    {

        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public bool NocRegistrationType { get; set; } = true;

        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string NocType { get; set; }


        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string SubTenantName { get; set; }


        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        [MaxLength(12, ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "MobileNumber")]
        [MinLength(6, ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "MobileNumber")]
        public string MobileNumber { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string CountryCode { get; set; }


        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "PropertyCodeRequired")]
        public string PropertyCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "PropertyValueRequired")]
        [StringLength(4, MinimumLength = 1, ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "PropertyCode")]
        public string PropertyValue { get; set; }



        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        //[EmailAddress(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "EmailAddress")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessageResourceType = typeof(ValidationRes),
        ErrorMessageResourceName = "EmailAddress")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string TradeLicenseIssuerId { get; set; }

        public string TradeLicenseType { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string TradeLicenseNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public DateTime ExpiryDate { get; set; }


        public string InitialApproval { get; set; }
        public string LicenseNo { get; set; }
        public string EmiratesId { get; set; }
        public string InputValueTradeLicenseNo { get; set; }
        [Required]
        public string Captcha { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!IsUae(CountryCode) || string.IsNullOrWhiteSpace(MobileNumber))
            {
                yield break;
            }

            var digitsOnly = new string(MobileNumber.Trim().Where(char.IsDigit).ToArray());

            // UAE local mobile: must be exactly 9 digits starting with 5 (5xxxxxxxx).
            // Leading zeros are NOT accepted (e.g. 0561147258 is invalid).
            const int uaeLocalLength = 9;
            if (digitsOnly.Length != uaeLocalLength || digitsOnly[0] != '5')
            {
                yield return new ValidationResult(
                    ValidationRes.UaeMobileMustStartWith5,
                    new[] { nameof(MobileNumber) });
            }
        }

        /// <summary>UAE when dial code is +971 / 971 or ISO country code AE.</summary>
        private static bool IsUae(string? code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            var c = code.Trim();
            return c.Equals("+971", StringComparison.Ordinal)
                || c.Equals("971", StringComparison.Ordinal)
                || c.Equals("AE", StringComparison.OrdinalIgnoreCase);
        }
    }
}