using FluentValidation;
using SSO_Irica.Application.DTOs.Auth.Requests;

namespace SSO_Irica.Application.Validators;

public sealed class VerifyTwoFactorRequestValidator : AbstractValidator<VerifyTwoFactorRequest>
{
    public VerifyTwoFactorRequestValidator()
    {
        RuleFor(x => x.NationalCode).NotEmpty().Must(RegisterRequestValidator.IsValidNationalCode);
        RuleFor(x => x.Code).Matches(@"^\d{6}$").WithMessage("OTP must contain six digits.");
    }
}
