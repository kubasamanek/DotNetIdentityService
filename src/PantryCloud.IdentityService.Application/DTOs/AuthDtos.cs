namespace PantryCloud.IdentityService.Application.DTOs;

public record RegisterRequestDto(string Email, string Password);
public record LoginRequestDto(string Email, string Password);
public record LoginResponseDto(string AccessToken, string RefreshToken);
public record RegisterResponseDto(string UserId);
public record RefreshTokenRequestDto(string RefreshToken);
public record RefreshTokenResponseDto(string AccessToken, string RefreshToken);
public record ForgotPasswordRequestDto(string Email);
public record ForgotPasswordResponseDto(string Token, string Url);
public record ResetPasswordRequestDto(string Email, string Token, string NewPassword);
public record ResetPasswordResponseDto();