using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using PantryCloud.IdentityService.Application;
using PantryCloud.IdentityService.Core.Entities;

namespace PantryCloud.IdentityService.Infrastructure.Services;

public sealed class TokenProvider : ITokenProvider
{
    private readonly IConfiguration _configuration;
    private readonly SigningCredentials _signingCredentials;

    public TokenProvider(IConfiguration configuration)
    {
        var privateKeyPath = configuration["Jwt:PrivateKeyPath"]!;
        var privateRsa = RSA.Create();
        var privateKey = File.ReadAllText(privateKeyPath);
        privateRsa.ImportFromPem(privateKey.ToCharArray());

        var rsaSecurityKey = new RsaSecurityKey(privateRsa)
        {
            KeyId = "key-id"
        };
        
        _signingCredentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256);
        _configuration = configuration;
    }

    public string CreateAccessToken(ApplicationUser user)
    {
        var handler = new JsonWebTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("email_verified", user.EmailVerified.ToString())
            ]),
            Expires = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpirationInMinutes")),
            SigningCredentials = _signingCredentials,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        return handler.CreateToken(tokenDescriptor);
    }

    public string CreateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }
}