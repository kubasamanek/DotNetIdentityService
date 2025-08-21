using PantryCloud.IdentityService.Core.Entities;

namespace PantryCloud.IdentityService.Application;

public interface ITokenProvider
{
    public string CreateAccessToken(ApplicationUser user);
    public string CreateRefreshToken();
}