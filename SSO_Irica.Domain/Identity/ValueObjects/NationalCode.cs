using System.Text.RegularExpressions;
using SSO_Irica.Domain.Common;

namespace SSO_Irica.Domain.Identity.ValueObjects;

public sealed record NationalCode
{
    public string Value { get; }

    private NationalCode(string value) => Value = value;

    public static NationalCode Create(string value)
    {
        value = value.Trim();
        if (!Regex.IsMatch(value, @"^\d{10}$"))
        {
            throw new DomainException("National code must contain exactly 10 digits.");
        }

        if (value.Distinct().Count() == 1)
        {
            throw new DomainException("National code is invalid.");
        }

        var checkDigit = value[9] - '0';
        var sum = Enumerable.Range(0, 9).Sum(index => (value[index] - '0') * (10 - index));
        var remainder = sum % 11;
        if (checkDigit != (remainder < 2 ? remainder : 11 - remainder))
        {
            throw new DomainException("National code is invalid.");
        }

        return new NationalCode(value);
    }

    public override string ToString() => Value;
}
