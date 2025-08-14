using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using PantryCloud.IdentityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PantryCloud.IdentityService.Application;
using PantryCloud.IdentityService.Application.Commands;
using PantryCloud.IdentityService.Infrastructure;
using PantryCloud.IdentityService.Infrastructure.Services;
using PantryCloud.IdentityService.Presentation;
using PantryCloud.IdentityService.Presentation.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPresentationLayerServices(builder.Configuration)
    .AddInfrastructureLayerServices(builder.Configuration)
    .AddApplicationLayerServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    await app.ApplyMigrationsAsync();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();