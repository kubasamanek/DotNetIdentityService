using MediatR;
using ErrorOr;
using PantryCloud.IdentityService.Application.DTOs;

namespace PantryCloud.IdentityService.Application.Commands.Handlers;

public class ForgotPasswordCommandHandler(IAuthService authService) : IRequestHandler<ForgotPasswordCommand, ErrorOr<ForgotPasswordResponseDto>>
{
    public async Task<ErrorOr<ForgotPasswordResponseDto>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        return await authService.ForgotPasswordAsync(request.Request, cancellationToken);
    }
}