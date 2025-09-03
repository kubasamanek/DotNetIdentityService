using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PantryCloud.IdentityService.Application;
using PantryCloud.IdentityService.Core;
using PantryCloud.IdentityService.Infrastructure.Persistence;
using PantryCloud.IdentityService.Infrastructure.Services;

namespace PantryCloud.IdentityService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayerServices(this IServiceCollection services, IConfiguration configuration)
    {
        var apiConfiguration = new ApiConfiguration();
        configuration.Bind(apiConfiguration);
        services.AddSingleton(apiConfiguration);
        
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddScoped<IAuthService, AuthService>();
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddAuthorization();
        services.AddJwtAuthentification(apiConfiguration);
        
        return services;
    }
    
    
    private static IServiceCollection AddJwtAuthentification(this IServiceCollection services, ApiConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var publicKeyPath = configuration.Jwt.PublicKeyPath;
                var publicRsa = RSA.Create();
                publicRsa.ImportFromPem(File.ReadAllText(publicKeyPath).ToCharArray());

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration.Jwt.Issuer,

                    ValidateAudience = true,
                    ValidAudience = configuration.Jwt.Audience,

                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new RsaSecurityKey(publicRsa)
                };
            });
        
        return services;
    }
}