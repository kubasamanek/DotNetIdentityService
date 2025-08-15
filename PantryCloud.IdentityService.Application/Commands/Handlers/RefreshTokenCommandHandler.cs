using MediatR;
using ErrorOr;
using PantryCloud.IdentityService.Application.DTOs;

namespace PantryCloud.IdentityService.Application.Commands.Handlers;

public sealed class RefreshTokenCommandHandler(IAuthService authService)
    : IRequestHandler<RefreshTokenCommand, ErrorOr<RefreshTokenResponseDto>>
{
    public async Task<ErrorOr<RefreshTokenResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await authService.RefreshTokenAsync(request.Request, cancellationToken);
    }
}