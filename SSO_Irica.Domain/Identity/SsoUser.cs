using SSO_Irica.Domain.Common;
using SSO_Irica.Domain.Identity.ValueObjects;

namespace SSO_Irica.Domain.Identity;

public sealed class SsoUser : AggregateRoot<Guid>
{
    public string NationalCodeValue { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string FullName => string.Join(' ', new[] { FirstName, LastName }.Where(x => !string.IsNullOrWhiteSpace(x)));
    public string Mobile { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public int? GenderId { get; private set; }
    public UserRole Role { get; private set; } = UserRole.User;

    private SsoUser() { }

    private SsoUser(NationalCode nationalCode, MobileNumber mobile, string fullName, string passwordHash)
    {
        Id = Guid.NewGuid();
        NationalCodeValue = nationalCode.Value;
        Mobile = mobile.Value;
        var parts = fullName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        FirstName = parts[0];
        LastName = parts.Length > 1 ? parts[1] : string.Empty;
        PasswordHash = passwordHash;
        Email = $"{nationalCode.Value}@local.invalid";
    }

    public static SsoUser Register(
        NationalCode nationalCode,
        MobileNumber mobile,
        string fullName,
        string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(fullName) || fullName.Trim().Length > 200)
        {
            throw new DomainException("Full name is required and must be at most 200 characters.");
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainException("Password hash is required.");
        }

        return new SsoUser(nationalCode, mobile, fullName, passwordHash);
    }

    public void SetRole(UserRole role) => Role = role;
    public void Deactivate() => IsActive = false;
}
