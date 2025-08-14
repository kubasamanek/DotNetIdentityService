using PantryCloud.IdentityService.Application.DTOs;

namespace PantryCloud.IdentityService.Application;

public interface IAuthService
{
    public Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken);
    public Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);
    public Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken);
}