using FluentValidation;
using SSO_Irica.Application.DTOs.Auth.Requests;

namespace SSO_Irica.Application.Validators;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.NationalCode).NotEmpty().Must(RegisterRequestValidator.IsValidNationalCode);
        RuleFor(x => x.Password).NotEmpty().MaximumLength(128);
    }
}
