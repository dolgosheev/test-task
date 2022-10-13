using System.ComponentModel.DataAnnotations;

using AutoMapper;

using Grpc.Core;

using Microsoft.AspNetCore.Mvc;

using Test.Rest.MapperProfiles;
using Test.Rest.Models.User;
using Test.Rest.ServiceConnectors;

using TestGrpc;

namespace Test.Rest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status504GatewayTimeout)]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;

        private readonly ITestGrpc _testGrpc;

        public UserController(IHostEnvironment env, ILogger<UserController> logger, ITestGrpc testGrpc)
        {
            _logger = logger;
            _testGrpc = testGrpc;

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
        }

        // Get user information by id
        [HttpGet]
        [Route("id/{userId:long}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserModelResponse))]
        public async Task<IActionResult> GetUserAsync([Required] long userId)
        {
            var response = await _testGrpc.GrpcClient.GetUserAsync(new GetUserRequest
            {
                UserId = userId
            }, new Metadata
            {
                new("X-Correlation-Id", Guid.NewGuid().ToString())
            });

            return response?.User is null
                ? BadRequest("Operation failed")
                : Ok(_mapper.Map<UserModelResponse>(response.User));
        }

        //Get user information by id
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserModelResponse>))]
        public async Task<IActionResult> GetUsersAsync()
        {
            var response = await _testGrpc.GrpcClient.GetUsersAsync(new GetUsersRequest(), new Metadata
            {
                new("X-Correlation-Id", Guid.NewGuid().ToString())
            });

            return response?.Users is null
                ? BadRequest("Operation failed")
                : Ok(_mapper.Map<List<UserModelResponse>>(response.Users));
        }
    }
}