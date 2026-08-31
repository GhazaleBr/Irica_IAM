using AutoMapper;
using SSO_Irica.Application.DTOs.Auth.Responses;
using SSO_Irica.Domain.Identity;

namespace SSO_Irica.Application.Mapping;

public sealed class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<SsoUser, UserResponse>()
            .ForCtorParam(nameof(UserResponse.NationalCode), o => o.MapFrom(x => x.NationalCodeValue))
            .ForCtorParam(nameof(UserResponse.Role), o => o.MapFrom(x => x.Role.ToString()));
    }
}
