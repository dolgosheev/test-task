using System.Net;

using Microsoft.AspNetCore.Server.Kestrel.Core;

using Serilog;
using Serilog.OpenTelemetry;

using Test.Rest.Extensions.SerilogEnricher;
using Test.Rest.ServiceConnectors;
using Test.Rest.Services;

namespace Test.Rest
{
    public static class Startup
    {
        // Config Host & Services
        internal static WebApplicationBuilder ConfigureHost(WebApplicationBuilder builder)
        {
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

                    listenOptions.Protocols = HttpProtocols.Http1;
                });
            });

            builder.Services.AddSingleton<ITestGrpc, ServiceConnectors.TestGrpc>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<PrometheusService>();
            return builder;
        }

        // Config App
        internal static WebApplication ConfigApp(WebApplication app, CancellationToken token)
        {
            using (var serviceScope = app.Services.GetService<IServiceScopeFactory>()?.CreateScope())
            {
                if (serviceScope != null)
                {
                    /* Prometheus */
                    var prometheus = serviceScope.ServiceProvider.GetRequiredService<PrometheusService>();
                    prometheus.Init(token);
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

            app.UseSerilogRequestLogging();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
    }
}