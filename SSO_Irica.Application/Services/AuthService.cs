using AutoMapper;
using FluentValidation;
using SSO_Irica.Application.Abstractions;
using SSO_Irica.Application.DTOs.Auth.Requests;
using SSO_Irica.Application.DTOs.Auth.Responses;
using SSO_Irica.Application.Exceptions;
using SSO_Irica.Application.Validators;
using SSO_Irica.Domain.Identity;
using SSO_Irica.Domain.Identity.ValueObjects;

namespace SSO_Irica.Application.Services;

public sealed class AuthService(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwt,
    IOtpService otp,
    IRefreshTokenService refreshTokens,
    IRoleAccessService roleAccess,
    IValidator<RegisterRequest> registerValidator,
    IValidator<LoginRequest> loginValidator,
    IValidator<VerifyTwoFactorRequest> verifyValidator,
    IMapper mapper) : IAuthService
{
    public async Task<UserResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        await registerValidator.ValidateAndThrowAsync(request, cancellationToken);
        var nationalCode = NationalCode.Create(request.NationalCode);
        if (await users.FindByNationalCodeAsync(nationalCode.Value, cancellationToken) is not null)
        {
            throw new AppException(ErrorCatalog.DuplicateUser, "An account with this national code already exists.", 409);
        }

        var user = SsoUser.Register(
            nationalCode,
            MobileNumber.Create(request.Mobile),
            request.FullName,
            passwordHasher.Hash(request.Password));
        await users.AddAsync(user, cancellationToken);
        await users.SaveChangesAsync(cancellationToken);
        return mapper.Map<UserResponse>(user);
    }

    public async Task<TwoFactorChallengeResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        await loginValidator.ValidateAndThrowAsync(request, cancellationToken);
        var user = await users.FindByNationalCodeAsync(request.NationalCode.Trim(), cancellationToken);
        if (user is null || !user.IsActive || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new AppException(ErrorCatalog.InvalidCredentials, "Invalid national code or password.", 401);
        }

        var developmentCode = await otp.SendAsync(user.Id, user.Mobile, cancellationToken);
        return new TwoFactorChallengeResponse("A verification code was generated.", 120, developmentCode);
    }

    public async Task<AuthSession> VerifyTwoFactorAsync(VerifyTwoFactorRequest request, CancellationToken cancellationToken)
    {
        await verifyValidator.ValidateAndThrowAsync(request, cancellationToken);
        var user = await users.FindByNationalCodeAsync(request.NationalCode.Trim(), cancellationToken)
            ?? throw new AppException(ErrorCatalog.InvalidOtp, "Invalid verification request.", 401);
        if (!await otp.VerifyAsync(user.Id, request.Code, cancellationToken))
        {
            throw new AppException(ErrorCatalog.InvalidOtp, "The verification code is invalid or has expired.", 401);
        }

        return new AuthSession(
            jwt.Create(user, (await roleAccess.GetUserAccessAsync(user.Id, cancellationToken))
                .Select(x => $"{x.ModuleId}:{x.PermissionId}")),
            await refreshTokens.IssueAsync(user.Id, cancellationToken),
            mapper.Map<UserResponse>(user));
    }

    public async Task<AuthSession?> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken)
    {
        var rotated = await refreshTokens.RotateAsync(refreshToken, cancellationToken);
        if (rotated is null)
        {
            return null;
        }

        var user = await users.FindByIdAsync(rotated.Value.UserId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            await refreshTokens.RevokeAsync(rotated.Value.NewRefreshToken, cancellationToken);
            return null;
        }

        return new AuthSession(
            jwt.Create(user, (await roleAccess.GetUserAccessAsync(user.Id, cancellationToken))
                .Select(x => $"{x.ModuleId}:{x.PermissionId}")),
            rotated.Value.NewRefreshToken,
            mapper.Map<UserResponse>(user));
    }

    public Task RevokeRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken) =>
        refreshTokens.RevokeAsync(refreshToken, cancellationToken);

    public async Task<UserResponse> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await users.FindByIdAsync(userId, cancellationToken)
            ?? throw new AppException(ErrorCatalog.UserNotFound, "User was not found.", 404);
        return mapper.Map<UserResponse>(user);
    }
}
