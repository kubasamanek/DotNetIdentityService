using MediatR;
using ErrorOr;
using PantryCloud.IdentityService.Application.DTOs;

namespace PantryCloud.IdentityService.Application.Commands;

public record LoginCommand(LoginRequestDto Request) : IRequest<ErrorOr<LoginResponseDto>>;