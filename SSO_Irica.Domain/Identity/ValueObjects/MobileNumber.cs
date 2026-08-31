using System.Text.RegularExpressions;
using SSO_Irica.Domain.Common;

namespace SSO_Irica.Domain.Identity.ValueObjects;

public sealed record MobileNumber
{
    public string Value { get; }

    private MobileNumber(string value) => Value = value;

    public static MobileNumber Create(string value)
    {
        value = value.Trim();
        if (!Regex.IsMatch(value, @"^09\d{9}$"))
        {
            throw new DomainException("Mobile number is invalid.");
        }

        return new MobileNumber(value);
    }

    public override string ToString() => Value;
}
