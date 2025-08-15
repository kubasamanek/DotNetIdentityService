using MediatR;
using ErrorOr;
using PantryCloud.IdentityService.Application.DTOs;

namespace PantryCloud.IdentityService.Application.Commands;

public record RegisterCommand(RegisterRequestDto Request) : IRequest<ErrorOr<RegisterResponseDto>>;