using Test.Rest;

var ctx = new CancellationTokenSource();

Startup
    .ConfigApp(
        Startup
            .ConfigureHost(
                WebApplication
                    .CreateBuilder(new WebApplicationOptions
                    {
                        Args = args
                    }))
            .Build(), ctx.Token
    )
    .Run();

ctx.Cancel();
ctx.Dispose();