using AutoMapper;
using UserAuthentication.Models;
using UserAuthentication.DTO_s;
using UserAuthentication.Data;
using UserAuthenticationApp.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Map between ApplicationUser and UserDto
        CreateMap<ApplicationUser, UserDto>();

        // Map between RegisterUser and ApplicationUser
        CreateMap<RegisterUser, ApplicationUser>();

        // Map between ApplicationUser and AuthModel
        CreateMap<ApplicationUser, AuthModel>()
            .ForMember(dest => dest.IsAuthenticated, opt => opt.MapFrom(src => true));

        // Map between UpdateUserDto and ApplicationUser
        CreateMap<UpdateUserDto, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));

        CreateMap<ApplicationUser, UpdateUserModel>()
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "User has been updated successfully."));
    }
}
