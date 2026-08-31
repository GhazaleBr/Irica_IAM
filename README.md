# Irica IAM

Independent ASP.NET Core 8 authentication service for the Portal and future applications.

## Layers

```text
IAM.Domain          # SsoUser aggregate, value objects and domain rules
IAM.Application     # use cases, DTOs, validators and ports
IAM.Infrastructure  # PostgreSQL, password hashing, OTP, SMS and JWT adapters
IAM.Api             # HTTP endpoints, CORS, authentication and exception handling
```

The service uses its own PostgreSQL database (`IricaIamDb`, فرآیند احراز و دسترسی)
with the IAM tables defined in the ERD: `Tbl_User`, `Tbl_Roles`,
`Tbl_User_Roles`, `Tbl_Permissions`, `Tbl_Role_Permissions`, `Tbl_Modules`,
`Tbl_Gender`, `Tbl_User_Positions`, `Tbl_Positions`,
`Tbl_Organization_Unit`, `Tbl_Organization_Type` and `Tbl_Actions_Log`.
Gallery tables are intentionally outside this IAM context and are not changed.
It intentionally has no dependency on the Portal projects.
The database adapter is isolated behind `IUserRepository`, so an Active Directory
adapter can be added later without changing the HTTP contract or domain rules.

## Endpoints

- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/verify-two-factor`
- `GET /api/auth/me`
- `GET /api/auth/admin`

The development SMS adapter logs the OTP and includes it only in Development responses.
Production must replace `ISmsSender` with a real provider and use a secret manager for
the JWT signing key and PostgreSQL password. Access tokens are short-lived and should
be kept only in frontend memory. Refresh tokens are rotated and sent only as an
`HttpOnly`, `Secure` cookie; they are never returned in JSON or stored in plaintext.

The frontend must send `credentials: "include"` when calling `/api/auth/refresh` and
`/api/auth/logout`.

## Run

```powershell
dotnet run --project SSO_Irica.Api
```

The assembly and solution display names are `IAM.*`; the physical folder remains
`SSO_Irica` for compatibility with existing project references.
Default local URL: `http://localhost:5300`.
