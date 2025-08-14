using Microsoft.Extensions.DependencyInjection;
using PantryCloud.IdentityService.Application.Commands;

namespace PantryCloud.IdentityService.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayerServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterCommand).Assembly));

        return services;
    }
}