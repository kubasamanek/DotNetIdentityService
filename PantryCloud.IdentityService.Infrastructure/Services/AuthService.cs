using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PantryCloud.IdentityService.Application;
using PantryCloud.IdentityService.Application.DTOs;
using PantryCloud.IdentityService.Core.Entities;
using PantryCloud.IdentityService.Infrastructure.Persistence;

namespace PantryCloud.IdentityService.Infrastructure.Services;

public class AuthService(
    TokenProvider tokenProvider,
    PasswordHasher passwordHasher,
    ApplicationDbContext dbContext,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to register user with email: {Email}", request.Email);

        if (await dbContext.Users.Exists(request.Email))
        {
            logger.LogWarning("Registration failed. User with email {Email} already exists.", request.Email);
            throw new InvalidOperationException("User already exists.");
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            EmailVerified = false,
            PasswordHash = passwordHasher.Hash(request.Password),
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User registered successfully with ID: {UserId}", user.Id);

        return new RegisterResponseDto(user.Id.ToString());
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        logger.LogInformation("User login attempt: {Email}", request.Email);

        var user = await dbContext.Users.GetByEmail(request.Email);

        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            logger.LogWarning("Login failed for email: {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var accessToken = tokenProvider.CreateAccessToken(user);
        var refreshToken = tokenProvider.CreateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} logged in successfully", user.Id);

        return new LoginResponseDto(accessToken, refreshToken);
    }

    public async Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to refresh token");

        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken, cancellationToken);

        if (user == null)
        {
            logger.LogWarning("Refresh token failed: token not found");
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            logger.LogWarning("Refresh token expired for user {UserId}", user.Id);
            throw new UnauthorizedAccessException("Refresh token expired.");
        }

        var newAccessToken = tokenProvider.CreateAccessToken(user);
        var newRefreshToken = tokenProvider.CreateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Refresh token succeeded for user {UserId}", user.Id);

        return new RefreshTokenResponseDto(newAccessToken, newRefreshToken);
    }
}