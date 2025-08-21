using MediatR;
using ErrorOr;
using PantryCloud.IdentityService.Application.DTOs;

namespace PantryCloud.IdentityService.Application.Commands.Handlers;

public class LoginCommandHandler(IAuthService authService) : IRequestHandler<LoginCommand, ErrorOr<LoginResponseDto>>
{
    public async Task<ErrorOr<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await authService.LoginAsync(request.Request, cancellationToken);
    }
}