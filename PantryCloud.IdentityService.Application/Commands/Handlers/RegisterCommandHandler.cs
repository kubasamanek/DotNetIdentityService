using MediatR;
using ErrorOr;
using FluentValidation;
using PantryCloud.IdentityService.Application.DTOs;

namespace PantryCloud.IdentityService.Application.Commands.Handlers;

public class RegisterCommandHandler(IAuthService authService) : IRequestHandler<RegisterCommand, ErrorOr<RegisterResponseDto>>
{
    public async Task<ErrorOr<RegisterResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return await authService.RegisterAsync(request.Request, cancellationToken);
    }
}