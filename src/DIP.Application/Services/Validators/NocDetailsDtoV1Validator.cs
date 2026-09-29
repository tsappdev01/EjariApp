using DIP.EServices;
using FluentValidation;
namespace DIP.Services.Validators;


public class NocDetailsDtoV1Validator : AbstractValidator<NocDetailsDtoV1>
{
    public NocDetailsDtoV1Validator()
    {
        // LicenseType is required ONLY when:
        // NOCFor != "1" AND Info.TypeName != "Individual"
        RuleFor(x => x.LicenseType)
            .NotEmpty()
            .WithMessage("License Type is required")
            .When(x => x.NOCFor != "1" && x.TypeName != "Individual")
            .WithErrorCode("LicenseType_Required");
    }
}
