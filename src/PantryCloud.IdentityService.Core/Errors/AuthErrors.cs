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

    public static Error UserDoesNotExist(string email) => Error.NotFound(
        code: "Auth.UserDoesNotExist",
        description: $"User with email {email} not found."
    );

    public static Error PasswordResetTokenNotValid = Error.Unauthorized(
        code: "Auth.PasswordResetTokenInvalid",
        description: $"Password reset token is not valid."
    );
    
    public static Error PasswordResetTokenExpired = Error.Unauthorized(
        code: "Auth.PasswordResetTokenExpired",
        description: $"Password reset token is expired."
    );
    
    public static Error PasswordResetTokenAlreadyUsed = Error.Unauthorized(
        code: "Auth.PasswordResetTokenAlreadyUsed",
        description: $"Password reset token has already been used."
    );
}