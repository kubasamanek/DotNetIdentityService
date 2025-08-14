using MediatR;
using PantryCloud.IdentityService.Application.DTOs;

namespace PantryCloud.IdentityService.Application.Commands;

public record RegisterCommand(RegisterRequestDto Request) : IRequest<RegisterResponseDto?>;