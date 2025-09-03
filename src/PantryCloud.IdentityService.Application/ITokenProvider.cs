using PantryCloud.IdentityService.Core.Entities;

namespace PantryCloud.IdentityService.Application;

public interface ITokenProvider
{
    string CreateAccessToken(ApplicationUser user);
    string CreateRefreshToken();
    string CreatePasswordResetToken();
    string CreateVerifyEmailToken();
}