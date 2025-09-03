using MediatR;
using ErrorOr;
using PantryCloud.IdentityService.Application.DTOs;

namespace PantryCloud.IdentityService.Application.Commands;

public record VerifyEmailCommand(VerifyEmailRequestDto Request) : IRequest<ErrorOr<VerifyEmailResponseDto>>;