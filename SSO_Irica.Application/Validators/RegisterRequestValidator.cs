using FluentValidation;
using SSO_Irica.Application.DTOs.Auth.Requests;

namespace SSO_Irica.Application.Validators;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NationalCode).NotEmpty().Must(IsValidNationalCode).WithMessage("National code is invalid.");
        RuleFor(x => x.Mobile).NotEmpty().Matches(@"^09\d{9}$").WithMessage("Mobile number is invalid.");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(16)
            .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain a lowercase letter.")
            .Matches(@"\d").WithMessage("Password must contain a digit.")
            .Matches(@"[^a-zA-Z\d]").WithMessage("Password must contain a symbol.");
    }

    public static bool IsValidNationalCode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || !System.Text.RegularExpressions.Regex.IsMatch(value, @"^\d{10}$") ||
            value.Distinct().Count() == 1)
        {
            return false;
        }

        var check = value[9] - '0';
        var sum = Enumerable.Range(0, 9).Sum(i => (value[i] - '0') * (10 - i));
        var remainder = sum % 11;
        return check == (remainder < 2 ? remainder : 11 - remainder);
    }
}
