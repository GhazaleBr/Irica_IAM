# Build from repository root:
# docker build -f SSO_Irica/SSO_Irica.Api/Dockerfile -t irica-iam:dev .
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY SSO_Irica/SSO_Irica.Domain/SSO_Irica.Domain.csproj SSO_Irica/SSO_Irica.Domain/
COPY SSO_Irica/SSO_Irica.Application/SSO_Irica.Application.csproj SSO_Irica/SSO_Irica.Application/
COPY SSO_Irica/SSO_Irica.Infrastructure/SSO_Irica.Infrastructure.csproj SSO_Irica/SSO_Irica.Infrastructure/
COPY SSO_Irica/SSO_Irica.Api/SSO_Irica.Api.csproj SSO_Irica/SSO_Irica.Api/
RUN dotnet restore SSO_Irica/SSO_Irica.Api/SSO_Irica.Api.csproj

COPY SSO_Irica SSO_Irica
RUN dotnet publish SSO_Irica/SSO_Irica.Api/SSO_Irica.Api.csproj \
    -c Release -o /app/publish --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "IAM.Api.dll"]
