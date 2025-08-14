using MediatR;
using PantryCloud.IdentityService.Application.DTOs;

namespace PantryCloud.IdentityService.Application.Commands.Handlers;

public sealed class RefreshTokenCommandHandler(IAuthService authService)
    : IRequestHandler<RefreshTokenCommand, RefreshTokenResponseDto?>
{
    public async Task<RefreshTokenResponseDto?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await authService.RefreshTokenAsync(request.Request, cancellationToken);
    }
}