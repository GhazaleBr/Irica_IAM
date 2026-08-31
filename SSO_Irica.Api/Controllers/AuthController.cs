using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSO_Irica.Application.Abstractions;
using SSO_Irica.Application.DTOs.Auth.Requests;
using SSO_Irica.Application.DTOs.Auth.Responses;
using SSO_Irica.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.RateLimiting;

namespace SSO_Irica.Api.Controllers;

[ApiController]
[Route("api/auth")]
[EnableRateLimiting("auth")]
public sealed class AuthController(
    IAuthService authService,
    IOptions<SsoSecurityOptions> securityOptions) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<UserResponse>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var user = await authService.RegisterAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created,
            user);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TwoFactorChallengeResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
        => Ok(await authService.LoginAsync(request, cancellationToken));

    [HttpPost("verify-two-factor")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> VerifyTwoFactor(
        VerifyTwoFactorRequest request,
        CancellationToken cancellationToken)
    {
        var session = await authService.VerifyTwoFactorAsync(request, cancellationToken);
        SetRefreshCookie(session.RefreshToken);
        return Ok(new AuthResponse(session.AccessToken, session.User));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Refresh(CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue(securityOptions.Value.RefreshTokenCookieName, out var refreshToken))
        {
            return Unauthorized(new { code = SSO_Irica.Application.Exceptions.ErrorCatalog.MissingRefreshToken, error = "Refresh token is missing." });
        }

        var session = await authService.RefreshAsync(refreshToken, cancellationToken);
        if (session is null)
        {
            DeleteRefreshCookie();
            return Unauthorized(new { code = SSO_Irica.Application.Exceptions.ErrorCatalog.InvalidRefreshToken, error = "Refresh token is invalid or expired." });
        }

        SetRefreshCookie(session.RefreshToken);
        return Ok(new AuthResponse(session.AccessToken, session.User));
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        if (Request.Cookies.TryGetValue(securityOptions.Value.RefreshTokenCookieName, out var refreshToken))
        {
            await authService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);
        }

        DeleteRefreshCookie();
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserResponse>> Me(CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized(new { code = SSO_Irica.Application.Exceptions.ErrorCatalog.InvalidToken, error = "Invalid access token." });
        }

        return Ok(await authService.GetCurrentUserAsync(userId, cancellationToken));
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public IActionResult AdminOnly() =>
        Ok(new { message = "Administrator access granted." });

    private void SetRefreshCookie(string value)
    {
        var settings = securityOptions.Value;
        Response.Cookies.Append(settings.RefreshTokenCookieName, value, new CookieOptions
        {
            HttpOnly = true,
            Secure = settings.SecureCookie,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            MaxAge = TimeSpan.FromDays(settings.RefreshTokenDays),
            IsEssential = true
        });
    }

    private void DeleteRefreshCookie()
    {
        var settings = securityOptions.Value;
        Response.Cookies.Delete(settings.RefreshTokenCookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = settings.SecureCookie,
            SameSite = SameSiteMode.Lax,
            Path = "/"
        });
    }
}
