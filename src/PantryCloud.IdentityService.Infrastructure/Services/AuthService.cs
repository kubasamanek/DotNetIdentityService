using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PantryCloud.IdentityService.Application;
using PantryCloud.IdentityService.Application.DTOs;
using PantryCloud.IdentityService.Core.Entities;
using PantryCloud.IdentityService.Infrastructure.Persistence;
using ErrorOr;
using Microsoft.Extensions.Configuration;
using PantryCloud.IdentityService.Core;
using PantryCloud.IdentityService.Core.Errors;

namespace PantryCloud.IdentityService.Infrastructure.Services;

public class AuthService(
    ITokenProvider tokenProvider,
    ApplicationDbContext dbContext,
    ILogger<AuthService> logger,
    ApiConfiguration config) : IAuthService
{
    public async Task<ErrorOr<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to register user with email: {Email}", request.Email);

        if (await dbContext.Users.Exists(request.Email))
        {
            logger.LogWarning("Registration failed. User with email {Email} already exists.", request.Email);
            return AuthErrors.RegistrationUserAlreadyExists(request.Email);
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            EmailVerified = false,
            PasswordHash = PasswordHasher.Hash(request.Password),
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User registered successfully with ID: {UserId}", user.Id);

        return new RegisterResponseDto(user.Id.ToString());
    }

    public async Task<ErrorOr<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        logger.LogInformation("User login attempt: {Email}", request.Email);

        var user = await dbContext.Users.GetByEmail(request.Email);

        if (user is null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
        {
            logger.LogWarning("Login failed for email: {Email}", request.Email);
            return AuthErrors.LoginFailed;
        }

        var accessToken = tokenProvider.CreateAccessToken(user);
        var refreshToken = tokenProvider.CreateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} logged in successfully", user.Id);

        return new LoginResponseDto(accessToken, refreshToken);
    }

    public async Task<ErrorOr<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to refresh token");

        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken, cancellationToken);

        if (user == null)
        {
            logger.LogWarning("Refresh token failed: token not found");
            return AuthErrors.InvalidRefreshToken;
        }

        if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            logger.LogWarning("Refresh token expired for user {UserId}", user.Id);
            return AuthErrors.ExpiredRefreshToken;
        }

        var newAccessToken = tokenProvider.CreateAccessToken(user);
        var newRefreshToken = tokenProvider.CreateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Refresh token succeeded for user {UserId}", user.Id);

        return new RefreshTokenResponseDto(newAccessToken, newRefreshToken);
    }

    public async Task<ErrorOr<ForgotPasswordResponseDto>> ForgotPasswordAsync(ForgotPasswordRequestDto request, CancellationToken cancellationToken)
    {
        logger.LogInformation("User {Email} forgot password.", request.Email);
        
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        
        if (user == null)
        {
            logger.LogWarning("Refresh token failed: token not found");
            return AuthErrors.UserDoesNotExist(request.Email);
        }

        var token = tokenProvider.CreatePasswordResetToken();
        var callbackUrl = $"{config.App.FrontendUrl}/reset-password?email={user.Email}&token={Uri.EscapeDataString(token)}";

        var entity = new ResetPasswordToken
        {
            Email = request.Email,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(1),
            Token = token,
            CallBackUrl = callbackUrl
        };
        
        await dbContext.ResetPasswordTokens.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        if (config.App.SendEmails)
        {
            var client = new SmtpClient(config.Email.Host, config.Email.Port);
            var message = new MailMessage
            {
                From = new MailAddress(config.Email.From),
                Subject = Constants.ResetPasswordEmailSubject,
                Body = string.Format(Constants.ResetPasswordEmailBodyTemplate, callbackUrl),
                IsBodyHtml = true
            };
            message.To.Add(user.Email);

            await client.SendMailAsync(message, cancellationToken);
        }

        return new ForgotPasswordResponseDto(token, callbackUrl);
    }

    public async Task<ErrorOr<ResetPasswordResponseDto>> ResetPasswordAsync(ResetPasswordRequestDto request, CancellationToken cancellationToken)
    {
        logger.LogInformation("User {Email} is trying to reset password.", request.Email);
                
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        
        if (user is null)
        {
            logger.LogWarning("Reset password failed: user not found");
            return AuthErrors.UserDoesNotExist(request.Email);
        }
        
        var token = await dbContext.ResetPasswordTokens
            .FirstOrDefaultAsync(t => t.Email == request.Email && t.Token == request.Token, cancellationToken);

        if (token is null)
        {
            logger.LogWarning("Reset password failed: token not found");
            return AuthErrors.PasswordResetTokenNotValid;
        }
        
        if (token.IsUsed)
        {
            logger.LogWarning("Reset password failed: token has been used");
            return AuthErrors.PasswordResetTokenAlreadyUsed;
        }
        
        if (token.IsExpired)
        {
            logger.LogWarning("Reset password failed: token has expired");
            return AuthErrors.PasswordResetTokenExpired;
        }

        user.PasswordHash = PasswordHasher.Hash(request.NewPassword);
        token.UsedAt =  DateTime.UtcNow;
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("User {Email} successfully reset password", request.Email);

        // TODO: Token cleanup (hosted service/cron job)
        
        return new ResetPasswordResponseDto();
    }
}