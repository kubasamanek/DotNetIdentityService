using Microsoft.EntityFrameworkCore;
using PantryCloud.IdentityService.Infrastructure.Persistence;

namespace PantryCloud.IdentityService.Presentation.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();
    }}