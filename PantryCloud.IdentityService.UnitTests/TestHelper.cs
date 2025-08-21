using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using PantryCloud.IdentityService.Application;
using PantryCloud.IdentityService.Core.Entities;
using PantryCloud.IdentityService.Infrastructure;
using PantryCloud.IdentityService.Infrastructure.Persistence;
using PantryCloud.IdentityService.Infrastructure.Services;

namespace PantryCloud.IdentityService.UnitTests;

internal static class TestHelper
{
    public static ApplicationDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new ApplicationDbContext(options);
    }

    public static ITokenProvider MockTokenProvider(string accessToken = "ACCESS_TOKEN",
        string refreshToken = "REFRESH_TOKEN")
    {
        var tp = Substitute.For<ITokenProvider>();
        tp.CreateAccessToken(Arg.Any<ApplicationUser>()).Returns(accessToken);
        tp.CreateRefreshToken().Returns(refreshToken);
        return tp;
    }

    public static ILogger<AuthService> MockLogger()
        => Substitute.For<ILogger<AuthService>>();

    public static ApplicationUser MakeUser(string email, string password, bool verified = false)
    {
        return new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            EmailVerified = verified,
            PasswordHash = PasswordHasher.Hash(password)
        };
    }
}