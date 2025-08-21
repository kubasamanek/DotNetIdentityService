using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PantryCloud.IdentityService.Application.Commands;
using PantryCloud.IdentityService.Application.Validators;

namespace PantryCloud.IdentityService.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayerServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterCommand).Assembly));

        services.AddValidatorsFromAssembly(typeof(RegisterCommand).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        return services;
    }
}