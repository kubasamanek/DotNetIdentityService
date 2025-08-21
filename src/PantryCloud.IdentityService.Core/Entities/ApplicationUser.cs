using System.ComponentModel.DataAnnotations;

namespace PantryCloud.IdentityService.Core.Entities;

public class ApplicationUser
{
    public Guid Id { get; init; }
    
    public required string Email { get; init; }
    
    public required string PasswordHash { get; init; }
    public bool EmailVerified { get; init; }
    
    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }
}
