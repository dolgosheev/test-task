using TestGrpc;

namespace Test.Rest.ServiceConnectors
{
    public interface ITestGrpc
    {
        TestGrpcService.TestGrpcServiceClient GrpcClient { get; }
    }
}