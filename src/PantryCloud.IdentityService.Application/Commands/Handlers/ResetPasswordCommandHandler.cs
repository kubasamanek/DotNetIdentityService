using MediatR;
using ErrorOr;
using PantryCloud.IdentityService.Application.DTOs;

namespace PantryCloud.IdentityService.Application.Commands.Handlers;

public class ResetPasswordCommandHandler(IAuthService authService) : IRequestHandler<ResetPasswordCommand, ErrorOr<ResetPasswordResponseDto>>
{
    public async Task<ErrorOr<ResetPasswordResponseDto>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        return await authService.ResetPasswordAsync(request.Request, cancellationToken);
    }
}