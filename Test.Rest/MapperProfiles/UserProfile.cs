using AutoMapper;

using Test.Rest.Models.User;

using TestGrpc;

namespace Test.Rest.MapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserEntity, UserModelResponse>()
                .ForMember(dst => dst.Id,
                    opt => opt.MapFrom(src => src.UserId))
                .ForMember(dst => dst.NickName,
                    opt => opt.MapFrom(src => src.UserNickName))
                .ForMember(dst => dst.FirstName,
                    opt => opt.MapFrom(src => src.UserFirstName))
                .ForMember(dst => dst.Age,
                    opt => opt.MapFrom(src => src.UserAge))
                ;
        }
    }
}