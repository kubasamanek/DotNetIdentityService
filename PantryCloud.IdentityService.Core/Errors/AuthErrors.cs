using ErrorOr;

namespace PantryCloud.IdentityService.Core.Errors;

public static class AuthErrors
{
    public static Error RegistrationUserAlreadyExists(string email) => Error.Conflict(
        code: "Auth.Registration.UserAlreadyExists",
        description: $"User with email {email} already exists."
    );

    public static Error LoginFailed = Error.Unauthorized(
        code: "Auth.Login.InvalidCredentials",
        description: "Invalid email or password."
    );

    public static Error InvalidRefreshToken = Error.Unauthorized(
        code: "Auth.RefreshToken.InvalidRefreshToken",
        description: "Invalid refresh token."
    );
    
    public static Error ExpiredRefreshToken = Error.Unauthorized(
        code: "Auth.RefreshToken.ExpiredRefreshToken",
        description: "Expired refresh token."
    );
}