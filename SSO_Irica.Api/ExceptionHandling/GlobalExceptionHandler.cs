using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using SSO_Irica.Application.Exceptions;
using SSO_Irica.Domain.Common;

namespace SSO_Irica.Api.ExceptionHandling;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled SSO exception for {Path}", httpContext.Request.Path);

        var error = exception switch
        {
            AppException appException => (appException.StatusCode, appException.Code, appException.Message),
            DomainException domainException => (StatusCodes.Status400BadRequest, ErrorCatalog.Validation, domainException.Message),
            ValidationException validationException => (StatusCodes.Status400BadRequest, ErrorCatalog.Validation,
                validationException.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed."),
            _ => (StatusCodes.Status500InternalServerError, ErrorCatalog.Internal, "An internal server error occurred.")
        };

        httpContext.Response.StatusCode = error.Item1;
        await httpContext.Response.WriteAsJsonAsync(new { code = error.Item2, error = error.Item3 }, cancellationToken);
        return true;
    }
}
