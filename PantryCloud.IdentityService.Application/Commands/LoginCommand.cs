using MediatR;
using PantryCloud.IdentityService.Application.DTOs;

namespace PantryCloud.IdentityService.Application.Commands;

public record LoginCommand(LoginRequestDto Request) : IRequest<LoginResponseDto?>;