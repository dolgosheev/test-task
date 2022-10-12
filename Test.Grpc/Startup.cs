using System.Net;

using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

using Serilog;
using Serilog.OpenTelemetry;

using Test.Grpc.DAL;
using Test.Grpc.Extensions.SerilogEnricher;
using Test.Grpc.ServiceInterfaces;
using Test.Grpc.Services;

namespace Test.Grpc;

// System configuration class
public static class Startup
{
    // interlock Sync flag
    public static int Sync;

    // Config Host & Services
    internal static WebApplicationBuilder ConfigureHost(WebApplicationBuilder builder)
    {
        var pgsqlHost = builder.Configuration.GetValue<string>("Postgres:Host");
        var pgsqlPort = builder.Configuration.GetValue<string>("Postgres:Port");
        var pgsqlUser = builder.Configuration.GetValue<string>("Postgres:User");
        var pgsqlPassword = builder.Configuration.GetValue<string>("Postgres:Password");
        var pgsqlDb = builder.Configuration.GetValue<string>("Postgres:Database");

        var connectionString =
            $"Host={pgsqlHost};Port={pgsqlPort};Database={pgsqlDb};Username={pgsqlUser};Password={pgsqlPassword};";

        // Database configuration
        builder.Services.AddDbContext<ApplicationContext>(context =>
            context.UseNpgsql(connectionString,
                opt => { opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); }
            )
        );

        // Logger config
        builder.Host.UseSerilog((context, lc) => lc
            .Enrich.WithCaller()
            .Enrich.WithResource(
                ("server", Environment.MachineName),
                ("app", AppDomain.CurrentDomain.FriendlyName))
            .WriteTo.Console()
            .ReadFrom.Configuration(context.Configuration)
        );

        // Kestrel config
        builder.WebHost.ConfigureKestrel((_, opt) =>
        {
            var host = builder.Configuration.GetValue<string>("App:Host");
            var port = builder.Configuration.GetValue<int>("App:Port");

            opt.Limits.MinRequestBodyDataRate = null;

            opt.Listen(IPAddress.Parse(host), port, listenOptions =>
            {
                Log.Information("The application [{AppName}] is successfully started at [{StartTime}] (UTC)",
                    AppDomain.CurrentDomain.FriendlyName,
                    DateTime.UtcNow.ToString("F"));

                listenOptions.Protocols = HttpProtocols.Http2;
            });
        });

        // Services collection
        builder.Services.AddScoped<IUser, UserService>();

        // gRPC config
        builder.Services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = true;
            options.IgnoreUnknownServices = false;
            options.MaxReceiveMessageSize = 4194304;
            options.MaxSendMessageSize = 4194304;
            options.EnableDetailedErrors = true;
        });

        return builder;
    }

    // Config App
    internal static WebApplication ConfigApp(WebApplication app, CancellationToken token)
    {
        // Init services
        using (var serviceScope = app.Services.GetService<IServiceScopeFactory>()?.CreateScope())
        {
            if (serviceScope != null)
            {
                // Postgres SQL migration
                var applicationContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationContext>();
                applicationContext.Database.Migrate();
            }
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            Log.ForContext("Mode", app.Environment.EnvironmentName);
            Log.Debug("App activated in [{Environment}] mode", app.Environment.EnvironmentName);
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints => { endpoints.MapGrpcService<GrpcService>(); });

        return app;
    }
}