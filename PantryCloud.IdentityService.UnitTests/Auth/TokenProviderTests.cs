using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using PantryCloud.IdentityService.Infrastructure.Services;
using Shouldly;

namespace PantryCloud.IdentityService.UnitTests.Auth;

public class TokenProviderTests
{
    [Fact]
    public async Task CreateAccessToken_IsValid_AndContainsExpectedClaims()
    {
        var provider = new TokenProvider(Constants.Jwt.Config);
        var token = provider.CreateAccessToken(Constants.ExampleUser);

        var handler = new JsonWebTokenHandler();
        var tvp = BuildValidationParams(Constants.Jwt.Secret, Constants.Jwt.Issuer, Constants.Jwt.Audience);
        var result = await handler.ValidateTokenAsync(token, tvp);

        result.IsValid.ShouldBeTrue(result.Exception?.ToString());

        var sub = result.ClaimsIdentity!.FindFirst(JwtRegisteredClaimNames.Sub)!;
        var email = result.ClaimsIdentity!.FindFirst(JwtRegisteredClaimNames.Email)!;
        var verified = result.ClaimsIdentity!.FindFirst("email_verified")!;

        sub.Value.ShouldBe(Constants.ExampleUser.Id.ToString());
        email.Value.ShouldBe(Constants.ExampleUser.Email);
        verified.Value.ShouldBe(Constants.ExampleUser.EmailVerified.ToString());
    }

    [Fact]
    public async Task CreateAccessToken_Expiration_IsInNearFuture()
    {
        var provider = new TokenProvider(Constants.Jwt.Config);
        var token = provider.CreateAccessToken(Constants.ExampleUser);

        var handler = new JsonWebTokenHandler();
        var tvp = BuildValidationParams(Constants.Jwt.Secret, Constants.Jwt.Issuer, Constants.Jwt.Audience);
        var result = await handler.ValidateTokenAsync(token, tvp);
        result.IsValid.ShouldBeTrue(result.Exception?.ToString());

        var jwt = handler.ReadJsonWebToken(token);
        var expUnix = long.Parse(jwt.Claims.First(c => c.Type == "exp").Value);
        var expUtc = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;

        expUtc.ShouldBeGreaterThan(DateTime.UtcNow);
        expUtc.ShouldBeLessThanOrEqualTo(DateTime.UtcNow.AddMinutes(Constants.Jwt.ExpirationInMinutes + 1));
    }

    [Fact]
    public async Task CreateAccessToken_FailsValidation_WithWrongSecret()
    {
        var provider = new TokenProvider(Constants.Jwt.Config);
        var token = provider.CreateAccessToken(Constants.ExampleUser);

        var handler = new JsonWebTokenHandler();
        var badTvp = BuildValidationParams(Constants.Jwt.BadSecret, Constants.Jwt.Issuer, Constants.Jwt.Audience);
        var validation = await handler.ValidateTokenAsync(token, badTvp);

        validation.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void CreateRefreshToken_IsBase64Of64Bytes()
    {
        var provider = new TokenProvider(Constants.Jwt.Config);
        var refresh = provider.CreateRefreshToken();

        Convert.FromBase64String(refresh).Length.ShouldBe(64);
    }
    
    private static TokenValidationParameters BuildValidationParams(string secret, string issuer, string audience)
        => new()
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(5)
        };

}