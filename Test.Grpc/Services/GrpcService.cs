using AutoMapper;

using Grpc.Core;

using Test.Grpc.MapperProfiles;
using Test.Grpc.ServiceInterfaces;

using TestGrpc;

namespace Test.Grpc.Services
{
    public class GrpcService : TestGrpcService.TestGrpcServiceBase
    {
        private readonly IMapper _mapper;
        private readonly IUser _user;

        public GrpcService(IHostEnvironment env, ILogger<GrpcService> logger, IUser user)
        {
            _user = user;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AllowNullCollections = true;
                cfg.AllowNullDestinationValues = true;
                cfg.AddProfile(typeof(UserProfile));
            });

            if (env.IsDevelopment())
            {
                config.CompileMappings();
                config.AssertConfigurationIsValid();
            }

            _mapper = new Mapper(config);

            logger.LogInformation("The service [{ServiceName}] is successfully started at [{StartTime}] (UTC)",
                nameof(GrpcService), DateTime.UtcNow.ToString("F"));
        }

        public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
        {
            var response = await _user.GetUserAsync(request.UserId);

            if (response is null) return new GetUserResponse();

            var map = _mapper.Map<UserEntity>(response);

            if (map is null) return new GetUserResponse();

            return _mapper.Map<GetUserResponse>(map);
        }

        public override async Task<GetUsersResponse> GetUsers(GetUsersRequest request, ServerCallContext context)
        {
            var response = await _user.GetUsersAsync();

            return response is null
                ? new GetUsersResponse()
                : new GetUsersResponse {Users = {_mapper.Map<IList<UserEntity>>(response)}};
        }
    }
}