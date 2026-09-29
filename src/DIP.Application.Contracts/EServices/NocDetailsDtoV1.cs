using DIP.Attributes;
using DIP.Validation;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DIP.EServices
{
    public class NocDetailsDtoV1 : NocDetailsDto
    {
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        //[Range(1, int.MaxValue, ErrorMessageResourceType = typeof(ValidationRes))]
        public decimal unitTotalArea { get; set; } = 0;
        [NocConditionalValidationAttribute(nameof(NOCFor), new[] { "1" }, [])]
        [ConditionalRangeForNoc(nameof(NOCFor), 1, int.MaxValue,
            ErrorMessage = "Floor Number must be greater than zero.")]
        public int FloorNumber { get; set; } = 0;

        [NocConditionalValidationAttribute(nameof(NOCFor), new[] { "1" }, [])]
        //[ConditionalRangeForNoc(nameof(NOCFor), 1, int.MaxValue,
        //ErrorMessage = "Number of rooms must be greater than zero.")]
        public int NumberOfRooms { get; set; } = 0;
        //[Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        [ConditionalRequiredCustom(nameof(NOCFor), "1", nameof(TypeName), "Individual",
            ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string LicenseType { get; set; } = "Professional";

        [NocConditionalValidationAttribute(nameof(NOCFor), new[] { "1", "2", "5", "6", "7" }, typeof(RequiredAttribute))]
        public string LicenseMainActivity { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        //[Range(1, int.MaxValue, ErrorMessageResourceType = typeof(ValidationRes))]
        public int SubLease { get; set; } = 0;
        public string SubleaseDIPStaffName { get; set; } = string.Empty;
        public string SubleaseBrokerID { get; set; } = string.Empty;
        //[Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string SubtenantDesignation { get; set; } = string.Empty;
        public string MaterialClassification { get; set; } = string.Empty;
        public bool IndustrialWaste { get; set; } = false;
        public string IndustrialWasteDescription { get; set; } = string.Empty;
        public bool Drainage { get; set; } = false;
        public string DrainageDescription { get; set; } = string.Empty;
        public bool Hazardous { get; set; } = false;
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string TenatCompanyName { get; set; } = string.Empty;
        public string TenantAddress { get; set; } = string.Empty;
        [Required(ErrorMessageResourceType = typeof(ValidationRes), ErrorMessageResourceName = "Required")]
        public string TenantCity { get; set; } = "Dubai";
        public string PropertyCategoryName { get; set; }
        public string PropertySubCategoryNames { get; set; }
        public bool ChangeInActivities { get; set; } = false;
        public int RenewalFlag { get; set; } = 0;
        public double ContractValue { get; set; } = 0;

        [NocConditionalValidationAttribute(nameof(NOCFor), ["1"], typeof(LessThanAttribute))]
        [ConditionalRangeForNoc(nameof(NOCFor), 0, int.MaxValue,
            ErrorMessage = "Utility Charge must be greater than zero.")]
        public double UtilityCharge { get; set; } = 0;
        public bool PropertyBillable { get; set; } = true;
        public string TypeName { get; set; } = string.Empty;
    }
}
