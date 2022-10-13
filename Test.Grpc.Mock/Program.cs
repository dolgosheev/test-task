using Grpc.Net.Client;

using TestGrpc;

using var channel = GrpcChannel.ForAddress("http://localhost:30001");
var client = new TestGrpcService.TestGrpcServiceClient(channel);

var getUser = client.GetUserAsync(new GetUserRequest
{
    UserId = 1
}, deadline: DateTime.UtcNow.AddMinutes(1));

Console.WriteLine($"User 1 is :\r\n{await getUser}");

var getUsers = client.GetUsersAsync(new GetUsersRequest(), deadline: DateTime.UtcNow.AddMinutes(1));

Console.WriteLine($"All Users List 1 is :\r\n{await getUsers}");

Console.WriteLine("Press any key to exit...");
Console.ReadKey();