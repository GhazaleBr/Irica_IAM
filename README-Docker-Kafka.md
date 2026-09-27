# IAM: Docker و Kafka

## ساخت و اجرا

از ریشه‌ی repository اجرا کنید:

```powershell
docker build -f SSO_Irica.Api/Dockerfile -t irica-iam:dev .
docker run --rm -p 5300:8080 `
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=IricaIamDb;Username=iam_app;Password=<secret>" `
  -e Jwt__Key="<minimum-32-character-secret>" `
  -e Jwt__Issuer="Irica.IAM" `
  -e Jwt__Audience="PortalUser.React" `
  -e Kafka__BootstrapServers="host.docker.internal:9092" `
  -e Kafka__Topic="iam-audit" `
  -e Audit__ServiceName="IAM" `
  -e Audit__ModuleId="<existing-iam-module-id>" `
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

## Kafka و لاگ امنیتی

IAM هیچ رمز عبور، JWT، OTP یا refresh token را در لاگ نمی‌نویسد. Middleware تمام درخواست‌ها
را ثبت می‌کند و برای رویدادهای حساس نام مشخص دارد:

```text
login
two_factor_verified
two_factor_failed
logout
token_refresh
نام اکشن کنترلر، برای مثال OrganizationManagement.CreatePosition
http_request (برای مسیرهای بدون اکشن کنترلر)
```

ساختار سند ارسالی برای تغییر یک واحد سازمانی:

```json
{
  "timestamp": "2026-08-26T10:00:00Z",
  "eventName": "OrganizationManagement.UpdateOrganizationUnit",
  "userId": "d183d3f8-154c-4a62-b9df-c56d8b870b07",
  "serviceName": "IAM",
  "actorType": "user",
  "actorSystem": "IAM",
  "entityType": "OrganizationUnit",
  "entityId": 42,
  "entityKey": null,
  "moduleId": 7,
  "moduleIds": null,
  "permissionIds": null,
  "applicationId": null,
  "positionId": null,
  "organizationUnitId": null,
  "organizationTypeId": 3,
  "parentOrganizationUnitId": 8,
  "subjectUserId": null,
  "traceId": "0HND3J5LDKJSQ:00000001",
  "action": "PUT /api/admin/iam/organization-units/42",
  "resource": "/api/admin/iam/organization-units/42",
  "statusCode": 200,
  "ipAddress": "127.0.0.1"
}
```

`Audit__ModuleId` اختیاری است؛ شناسهٔ واقعی ماژول IAM در `Tbl_Modules` را وارد کنید.
اگر ماژول هنوز ساخته نشده، مقدار پیش‌فرض صفر باعث توقف سرویس نمی‌شود و `moduleId` برای اکشن‌های
بدون ماژول مشخص، null ثبت می‌شود. `moduleId` برای عملیات روی خود ماژول، شناسهٔ همان ماژول است؛
در سایر عملیات شناسهٔ ماژول IAM تنظیم‌شده است. `entityId` معادل عددی `Fld_Entity_Id` در تصویر است.
برای موجودیت‌هایی با شناسهٔ GUID مثل `Tbl_User`، مقدار GUID در `entityKey` قرار می‌گیرد.
برای تغییر دسترسی چند ماژول، `moduleIds` و `permissionIds` همهٔ شناسه‌های درخواست را نگه می‌دارند؛
شناسه‌های مرتبط با سمت، واحد، نوع سازمان و واحد والد نیز در فیلدهای مربوط ثبت می‌شوند.
`subjectUserId` شناسهٔ کاربری است که نقش یا سمت به او تخصیص داده شده است.
`userId` همیشه شناسهٔ کاربری است که درخواست را انجام داده است، نه کاربرِ هدف.
در درخواست ناشناس ناموفق، هویت کاربر قابل احراز نیست و `userId` برابر null می‌ماند.
`serviceName` نام سرویس اجراکننده است؛ اگر JWT معتبر claim `client_id` یا `azp` داشته باشد،
`actorSystem` آن client است، وگرنه نام سرویس IAM ثبت می‌شود. JWT فعلی IAM این claimها را تولید نمی‌کند.

برای خواندن رویدادها، یک consumer با دسترسی محدود به topic بسازید:

```text
kafka-console-consumer --bootstrap-server localhost:9092 --topic iam-audit --from-beginning
```

تمام درخواست‌های API، شامل خواندن، تغییر، خطای اعتبارسنجی، عدم دسترسی و خطای سرور،
در یک topic منتشر می‌شوند. متن درخواست، کوکی و توکن ثبت نمی‌شوند. انتشار تا دریافت تاییدیه Kafka
منتظر می‌ماند؛ producer از ارسال idempotent استفاده می‌کند و برای هر پیام تا پنج ثانیه تلاش می‌کند.
اگر broker قطع باشد، درخواست اصلی ادامه می‌یابد و خطا در لاگ داخلی نوشته می‌شود؛ در این حالت رویداد
ممکن است از دست برود. برای تضمین ثبت همه تغییرات حتی هنگام قطعی، transactional outbox لازم است.
Kafka ابزار جستجوی صفحه‌بندی‌شده نیست؛ endpoint قدیمی `/api/admin/audit/events` حذف شده است.
در محیط عملیاتی topic را از قبل بسازید، retention مناسب تنظیم کنید و دسترسی producer و consumer را محدود کنید.
