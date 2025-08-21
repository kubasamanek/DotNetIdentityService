using PantryCloud.IdentityService.Application.DTOs;
using ErrorOr;

namespace PantryCloud.IdentityService.Application;

public interface IAuthService
{
    public Task<ErrorOr<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken);
    public Task<ErrorOr<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);
    public Task<ErrorOr<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken);
}