using AutoMapper;
using HPVTesting.Business.ViewModels;
using HPVTesting.Domain.Models;

namespace HPVTesting.Business.Helpers
{
    public class MapperConfiguration : Profile
    {
        public MapperConfiguration()
        {
            CreateMap<UserModel, User>();

            CreateMap<User, UserModel>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AspNetUser.Email));

            CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AspNetUser.Email))
                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => src.AspNetUser.EmailConfirmed))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name));

            CreateMap<UserSocialConnection, UserSocialConnectionModel>();
            CreateMap<UserSocialConnectionModel, UserSocialConnection>();

        }
    }
}