using Microsoft.AspNetCore.Identity;

namespace PantryCloud.IdentityService.Core.Entities;

public class ApplicationUser 
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public bool EmailVerified { get; set; }
    
    public string? RefreshToken { get; set; }
    
    public DateTime? RefreshTokenExpiryTime { get; set; }
