using MediatR;
using PantryCloud.IdentityService.Application.DTOs;

namespace PantryCloud.IdentityService.Application.Commands.Handlers;

public class LoginCommandHandler(IAuthService authService) : IRequestHandler<LoginCommand, LoginResponseDto?>
{
    
    public async Task<LoginResponseDto?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await authService.LoginAsync(request.Request, cancellationToken);
    }
}