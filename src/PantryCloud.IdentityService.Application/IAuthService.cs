using PantryCloud.IdentityService.Application.DTOs;
using ErrorOr;

namespace PantryCloud.IdentityService.Application;

public interface IAuthService
{
    Task<ErrorOr<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken);
    Task<ErrorOr<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);
    Task<ErrorOr<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken);
    Task<ErrorOr<ForgotPasswordResponseDto>> ForgotPasswordAsync(ForgotPasswordRequestDto request, CancellationToken cancellationToken);
    Task<ErrorOr<ResetPasswordResponseDto>> ResetPasswordAsync(ResetPasswordRequestDto request, CancellationToken cancellationToken);
    Task<ErrorOr<VerifyEmailResponseDto>> VerifyEmailAsync(VerifyEmailRequestDto request, CancellationToken cancellationToken);
}