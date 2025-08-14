using MediatR;
using PantryCloud.IdentityService.Application.DTOs;

namespace PantryCloud.IdentityService.Application.Commands;

public record RefreshTokenCommand(RefreshTokenRequestDto Request) : IRequest<RefreshTokenResponseDto?>;
