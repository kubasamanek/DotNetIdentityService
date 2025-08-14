using MediatR;
using PantryCloud.IdentityService.Application.DTOs;

namespace PantryCloud.IdentityService.Application.Commands.Handlers;

public class RegisterCommandHandler(IAuthService authService) : IRequestHandler<RegisterCommand, RegisterResponseDto?>
{
    public async Task<RegisterResponseDto?> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return await authService.RegisterAsync(request.Request, cancellationToken);
    }
}