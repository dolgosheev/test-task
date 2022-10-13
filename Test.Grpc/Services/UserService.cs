using Microsoft.EntityFrameworkCore;

using Test.Grpc.DAL;
using Test.Grpc.DAL.Entities;
using Test.Grpc.ServiceInterfaces;

namespace Test.Grpc.Services
{
    public class UserService : IUser
    {
        private readonly ApplicationContext _ctx;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationContext ctx, ILogger<UserService> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        public async Task<User?> GetUserAsync(long? userId)
        {
            if (userId is null || _ctx.Users is null) return null;

            while (Interlocked.Exchange(ref Startup.Sync, 1) == 0)
                Thread.Sleep(10);

            try
            {
                var result = await _ctx.Users.FindAsync(userId);
                if (result is not null)
                    return result;
            }
            catch (Exception e)
            {
                _logger.LogWarning("An error was occured {Exception}", e.Message);
            }
            finally
            {
                Interlocked.Exchange(ref Startup.Sync, 0);
            }

            _logger.LogError("Could not execute {Method}", nameof(GetUserAsync));
            return null;
        }

        public async Task<List<User>?> GetUsersAsync()
        {
            if (_ctx.Users is null) return null;

            while (Interlocked.Exchange(ref Startup.Sync, 1) == 0)
                Thread.Sleep(10);

            try
            {
                return await _ctx.Users.ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("An error was occured {Exception}", e.Message);
            }
            finally
            {
                Interlocked.Exchange(ref Startup.Sync, 0);
            }

            _logger.LogError("Could not execute {Method}", nameof(GetUsersAsync));
            return null;
        }
    }
}