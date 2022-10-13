using AutoMapper;

using Test.Grpc.DAL.Entities;

using TestGrpc;

namespace Test.Grpc.MapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserEntity>()
                .ForPath(dst => dst.UserId,
                    opt => opt.MapFrom(src => src.Id))
                .ForPath(dst => dst.UserNickName,
                    opt => opt.MapFrom(src => src.NickName))
                .ForPath(dst => dst.UserFirstName,
                    opt => opt.MapFrom(src => src.FirstName))
                .ForPath(dst => dst.UserAge,
                    opt => opt.MapFrom(src => src.Age))
                ;

            CreateMap<UserEntity, GetUserResponse>()
                .ForPath(dst => dst.User.UserId,
                    opt => opt.MapFrom(src => src.UserId))
                .ForPath(dst => dst.User.UserNickName,
                    opt => opt.MapFrom(src => src.UserNickName))
                .ForPath(dst => dst.User.UserFirstName,
                    opt => opt.MapFrom(src => src.UserFirstName))
                .ForPath(dst => dst.User.UserAge,
                    opt => opt.MapFrom(src => src.UserAge))
                ;
        }
    }
}