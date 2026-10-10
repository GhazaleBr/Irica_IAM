# Development administrator

In `Development`, `DevelopmentAdmin:Enabled=true` provisions a full administrator
in the configured local IAM PostgreSQL database. Startup applies pending migrations,
aligns the identity sequences with existing rows, and seeds missing data inside a
transaction protected by a PostgreSQL advisory lock. Repeated startup reuses the
account and records. Set `DevelopmentAdmin__Enabled=false` to stop provisioning;
this does not delete the existing account or revoke its grants.

- National code: `9876543210` (synthetic development account).
- Password: generated randomly on first startup, stored in
  `SSO_Irica.Api/.local/development-admin.json`. This directory is ignored by Git.
- Existing users and passwords are not replaced. If the national code already exists
  without the local credentials file, configure an unused `DevelopmentAdmin:NationalCode`.
- Do not delete the credentials file: it is reused and is not regenerated for existing accounts.

The seed creates or reuses the `Admin` role, assigns it to the development account,
and enables all module/permission combinations for that role, including existing
catalog entries. It creates IAM and Portal application/module entries and standard
read/create/update/delete/manage permissions. Existing members of the Admin role
therefore also receive the expanded catalog grants in this development database.
It also creates a development organization type, root unit, administrator position,
and user-position assignment. New catalog entries are included on the next startup.

## Login and Portal usage

1. Run IAM in Development.
2. POST `/api/auth/login` with `nationalCode` and `password` from the local file.
3. POST `/api/auth/verify-two-factor` with `nationalCode` and the `developmentCode`
   returned by login (as the `code` field).
4. Use the returned `accessToken` as `Authorization: Bearer <token>` in both IAM and Portal Admin.

Portal Admin already accepts IAM's `Admin` role. Its Development configuration
uses the same issuer (`Irica.IAM`), audience (`PortalUser.React`) and signing key
as IAM. No `province_id` restriction is issued, so the account has central access
to all current Portal admin controllers, including provincial resources and media.
Database constraints and required request fields still apply.

There is no city entity or city endpoint in either inspected project. Provinces
are managed in Portal Admin. This seed creates IAM organization data, not Portal
content or geographic sample records in the separate Portal database.

The provisioning code never runs outside Development and rejects non-local
PostgreSQL hosts. The created account and grants persist in this local development
database; do not reuse it or its generated credentials for production.
