namespace SSO_Irica.Application.Exceptions;

public static class ErrorCatalog
{
    public const string DuplicateUser = "SSO-USER-409";
    public const string InvalidCredentials = "SSO-AUTH-401";
    public const string InvalidOtp = "SSO-OTP-401";
    public const string UserNotFound = "SSO-USER-404";
    public const string Validation = "SSO-VALIDATION-400";
    public const string DatabaseUnavailable = "SSO-DB-503";
    public const string Internal = "SSO-500";
    public const string MissingRefreshToken = "SSO-REFRESH-400";
    public const string InvalidRefreshToken = "SSO-REFRESH-401";
    public const string InvalidToken = "SSO-AUTH-401";
    public const string OrganizationNotFound = "IAM-ORG-404";
    public const string PositionNotFound = "IAM-POSITION-404";
    public const string OrganizationTypeNotFound = "IAM-ORGTYPE-404";
    public const string GenderNotFound = "IAM-GENDER-404";
    public const string DuplicateCatalogCode = "IAM-CATALOG-409";
    public const string InvalidRelationship = "IAM-RELATION-400";
    public const string ApplicationNotFound = "IAM-APPLICATION-404";
}
