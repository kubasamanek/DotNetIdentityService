using AutoMapper;
using PantryCloud.IdentityService.Application.Commands;
using PantryCloud.IdentityService.Application.DTOs;

namespace PantryCloud.IdentityService.Presentation.Mapping;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<RegisterRequestDto, RegisterCommand>()
            .ConstructUsing(dto => new RegisterCommand(dto));
        
        CreateMap<RefreshTokenRequestDto, RefreshTokenCommand>()
            .ConstructUsing(dto => new RefreshTokenCommand(dto));
        
        CreateMap<LoginRequestDto, LoginCommand>()
            .ConstructUsing(dto => new LoginCommand(dto));
    }
}