using DIP.Validation;
using System.ComponentModel.DataAnnotations;

namespace DIP.EServices
{
    //public class ForgetDto 
    //{

    //    [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
    //    public string ReferenceNumber { get; set; }

    //    [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
    //    [MaxLength(9, ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "MaxLength")]
    //    [MinLength(9, ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "MaxLength")]
    //    public string MobileNumber { get; set; }

    //    [EmailAddress(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "InvalidEmailFormat")]
    //    public string Email { get; set; }

    //    public bool ChangeContact { get; set; }

    //    [Required]
    //    public string Captcha { get; set; }

    //    // Custom validation to ensure either MobileNumber or Email is required based on ChangeContact
    //    public class ValidateContact : ValidationAttribute
    //    {
    //        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //        {
    //            var model = (ForgetDto)validationContext.ObjectInstance;

    //            if (!model.ChangeContact)
    //            {
    //                if (string.IsNullOrEmpty(model.MobileNumber))
    //                {
    //                    return new ValidationResult("Mobile number is required.");
    //                }
    //            }
    //            else
    //            {
    //                if (string.IsNullOrEmpty(model.Email))
    //                {
    //                    return new ValidationResult("Email is required.");
    //                }
    //            }

    //            return ValidationResult.Success;
    //        }
    //    }

    //    // Applying the custom validation
    //    [ValidateContact(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "ContactValidation")]
    //    public object Contact { get; set; }
    //}

    public class ForgetDto
    {
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string ReferenceNumber { get; set; }

        // Conditional validation for MobileNumber or Email based on ChangeContact
        [ConditionalRequired("ChangeContact", false)]
        [MaxLength(14, ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "MobileNumber")]
        [MinLength(6, ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "MobileNumber")]
        public string MobileNumber { get; set; }

        [ConditionalRequired("ChangeContact", true)]
        //[EmailAddress(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "InvalidEmailFormat")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessageResourceType = typeof(ValidationRes),
        ErrorMessageResourceName = "EmailAddress")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public bool ChangeContact { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string CountryCode { get; set; } = "+971";

        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string Captcha { get; set; }

        public class ConditionalRequired : ValidationAttribute
        {
            private readonly string _propertyName;
            private readonly object _expectedValue;

            public ConditionalRequired(string propertyName, object expectedValue)
            {
                _propertyName = propertyName;
                _expectedValue = expectedValue;
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (ForgetDto)validationContext.ObjectInstance;
                var property = model.GetType().GetProperty(_propertyName);
                var propertyValue = property.GetValue(model);

                // Check if the property value matches the expected value for the condition to apply
                if (propertyValue != null && propertyValue.Equals(_expectedValue))
                {
                    // If the condition matches, the field is required
                    if (string.IsNullOrEmpty(value?.ToString()))
                    {
                        // Return a resource-based error message

                        return new ValidationResult(
                                $"The {validationContext.MemberName} field is required.",
                                [validationContext.MemberName!]
                            );
                    }
                }

                // If the condition doesn't match, don't apply validation
                return ValidationResult.Success;
            }
        }
    }

}