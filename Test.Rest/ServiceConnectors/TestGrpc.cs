using Grpc.Net.Client;

using TestGrpc;

namespace Test.Rest.ServiceConnectors;

public class TestGrpc : TestGrpcService.TestGrpcServiceClient, ITestGrpc
{
    private readonly GrpcChannel _channel;
    private readonly ILogger<TestGrpc> _logger;
    private TestGrpcService.TestGrpcServiceClient? _instance;

    public TestGrpc(IConfiguration configuration, ILogger<TestGrpc> logger)
    {
        _logger = logger;
        var host = configuration.GetValue<string>("TestGrpc:Host");
        var port = configuration.GetValue<string>("TestGrpc:Port");

        var serviceAddress = string.Concat("http://", host, ":", port);

        _channel = GrpcChannel.ForAddress(serviceAddress);
    }

    public TestGrpcService.TestGrpcServiceClient GrpcClient
    {
        get
        {
            if (_instance is not null) return _instance;

            _instance = new TestGrpcService.TestGrpcServiceClient(_channel);
            _logger.LogInformation("{Connector} instance created", nameof(TestGrpc));

            return _instance;
        }
    }
}