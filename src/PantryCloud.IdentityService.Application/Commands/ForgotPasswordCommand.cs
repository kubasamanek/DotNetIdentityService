using MediatR;
using ErrorOr;
using PantryCloud.IdentityService.Application.DTOs;

namespace PantryCloud.IdentityService.Application.Commands;

public record ForgotPasswordCommand(ForgotPasswordRequestDto Request) : IRequest<ErrorOr<ForgotPasswordResponseDto>>;