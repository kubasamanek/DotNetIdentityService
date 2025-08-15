using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PantryCloud.IdentityService.Application.Commands;
using PantryCloud.IdentityService.Application.DTOs;

namespace PantryCloud.IdentityService.Presentation.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator, IMapper mapper) : ApiControllerBase(mediator, mapper)
{

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var command = Mapper.Map<LoginCommand>(request);
        var result = await Mediator.Send(command, cancellationToken);

        return FromResult(result);
    }
  
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(RefreshTokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        var command = Mapper.Map<RefreshTokenCommand>(request);
        var result = await Mediator.Send(command, cancellationToken);

        return FromResult(result);

    }
    
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken cancellationToken)
    {
        var command = Mapper.Map<RegisterCommand>(request);
        var result = await Mediator.Send(command, cancellationToken);

        return FromResult(result);
    }
}