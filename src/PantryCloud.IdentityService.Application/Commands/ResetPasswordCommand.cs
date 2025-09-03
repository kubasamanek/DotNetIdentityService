using MediatR;
using ErrorOr;
using PantryCloud.IdentityService.Application.DTOs;

namespace PantryCloud.IdentityService.Application.Commands;

public record ResetPasswordCommand(ResetPasswordRequestDto Request) : IRequest<ErrorOr<ResetPasswordResponseDto>>;