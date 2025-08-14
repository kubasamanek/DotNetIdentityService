using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PantryCloud.IdentityService.Application;
using PantryCloud.IdentityService.Application.Commands;
using PantryCloud.IdentityService.Application.DTOs;
using PantryCloud.IdentityService.Core.Entities;
using PantryCloud.IdentityService.Infrastructure.Persistence;

namespace PantryCloud.IdentityService.Infrastructure.Services;

public class AuthService(TokenProvider tokenProvider, 
    PasswordHasher passwordHasher, 
    ApplicationDbContext dbContext,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken)
    {
        if (await dbContext.Users.Exists(request.Email))
        {
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
        
        return new RegisterResponseDto(user.Id.ToString());
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.GetByEmail(request.Email);

        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var accessToken = tokenProvider.CreateAccessToken(user);

        var refreshToken = tokenProvider.CreateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new LoginResponseDto(accessToken, refreshToken);
    }

    public async Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken, cancellationToken);

        logger.LogInformation("Refresh Token: {RefreshToken}", request.RefreshToken);
        logger.LogInformation("User is " + user?.Id);
        
        if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        var newAccessToken = tokenProvider.CreateAccessToken(user);
        var newRefreshToken = tokenProvider.CreateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResponseDto(newAccessToken, newRefreshToken);
    }
}