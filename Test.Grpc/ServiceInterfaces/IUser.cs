using Test.Grpc.DAL.Entities;

namespace Test.Grpc.ServiceInterfaces;

public interface IUser
{
    Task<User?> GetUserAsync(long? userId);
    Task<List<User>?> GetUsersAsync();
}