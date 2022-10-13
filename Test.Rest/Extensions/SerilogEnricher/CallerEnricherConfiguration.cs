using Serilog;
using Serilog.Configuration;

namespace Test.Rest.Extensions.SerilogEnricher
{
    public static class CallerEnrichmentConfiguration
    {
        public static LoggerConfiguration WithCaller(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            return enrichmentConfiguration.With<CallerEnricher>();
        }
    }
}