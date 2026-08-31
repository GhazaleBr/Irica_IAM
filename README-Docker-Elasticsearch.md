# IAM: Docker و Elasticsearch

## ساخت و اجرا

از ریشه‌ی repository اجرا کنید:

```powershell
docker build -f SSO_Irica/SSO_Irica.Api/Dockerfile -t irica-iam:dev .
docker run --rm -p 5300:8080 `
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=IricaIamDb;Username=iam_app;Password=<secret>" `
  -e Jwt__Key="<minimum-32-character-secret>" `
  -e Jwt__Issuer="Irica.IAM" `
  -e Jwt__Audience="PortalUser.React" `
  -e Elasticsearch__Url="https://elasticsearch.example.com:9200" `
  -e Elasticsearch__Index="iam-audit" `
  -e Elasticsearch__ApiKey="<elastic-api-key>" `
  irica-iam:dev
```

در محیط production رمزها را در command history یا فایل image قرار ندهید؛ از Docker Secrets،
Kubernetes Secrets یا secret manager استفاده کنید. مقدار `Jwt__Key` باید حداقل ۳۲ کاراکتر
تصادفی و خارج از source control باشد.

## جدول‌های ERD

`Tbl_Application` شامل `Fld_Id`، `Fld_Code`، `Fld_Title` و `Fld_IsActive` است.
`Tbl_Modules` شامل `Fld_Id`، `Fld_Code`، `Fld_Title` و `Fld_Application_Id` است.
هر Module دقیقاً به یک Application فعال وابسته است و حذف فیزیکی انجام نمی‌شود؛
Application با `Fld_IsActive=false` غیرفعال می‌شود.

Migration جدید:

```powershell
dotnet ef database update `
  --project .\SSO_Irica\SSO_Irica.Infrastructure\SSO_Irica.Infrastructure.csproj `
  --startup-project .\SSO_Irica\SSO_Irica.Api\SSO_Irica.Api.csproj `
  --context SsoDbContext
```

## APIهای Application و Module

همه‌ی endpointهای مدیریتی نیازمند JWT و Role=`Admin` هستند:

```text
GET    /api/admin/access/applications
POST   /api/admin/access/applications
PUT    /api/admin/access/applications/{id}
PATCH  /api/admin/access/applications/{id}/active?value=false

GET    /api/admin/access/modules?applicationId=1
POST   /api/admin/access/modules
PUT    /api/admin/access/modules/{id}
```

نمونه‌ی درخواست:

```json
{
  "code": "PORTAL",
  "title": "پرتال خبری"
}
```

```json
{
  "code": "NEWS",
  "title": "اخبار",
  "applicationId": 1
}
```

## Elasticsearch و لاگ امنیتی

IAM هیچ رمز عبور، JWT، OTP یا refresh token را در لاگ نمی‌نویسد. Middleware تمام درخواست‌ها
را ثبت می‌کند و برای رویدادهای حساس نام مشخص دارد:

```text
login
two_factor_verified
logout
token_refresh
http_request
```

ساختار سند ارسالی:

```json
{
  "timestamp": "2026-08-26T10:00:00Z",
  "eventName": "login",
  "userId": null,
  "action": "POST /api/auth/login",
  "resource": "/api/auth/login",
  "statusCode": 200,
  "ipAddress": "127.0.0.1"
}
```

گزارش‌گیری فقط برای Admin:

```text
GET /api/admin/audit/events?page=1&pageSize=50&eventName=login
GET /api/admin/audit/events?userId=<guid>&from=2026-08-01T00:00:00Z
```

اتصال Elasticsearch با `HttpClientFactory`، timeout کوتاه و API Key انجام می‌شود. اگر Elasticsearch
موقتاً در دسترس نباشد، درخواست اصلی کاربر fail نمی‌شود و خطا فقط در لاگ داخلی برنامه ثبت می‌گردد.
برای production بهتر است Elasticsearch پشت TLS باشد، API Key محدود به index مخصوص IAM باشد و
دسترسی index با role محدود شود.
