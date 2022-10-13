using Prometheus;

namespace Test.Rest.Services
{
    public class PrometheusService
    {
        private readonly ILogger<PrometheusService> _logger;

        private readonly Counter _processedJobCount = Metrics
            .CreateCounter("app_jobs_processed_total", "Number of processed jobs.");

        private readonly Counter _tickTock =
            Metrics.CreateCounter("app_ticks_total", "Just keeps on ticking");

        public PrometheusService(IConfiguration configuration, ILogger<PrometheusService> logger)
        {
            Configuration = configuration;
            _logger = logger;

            var prometheusPort = Configuration.GetValue<int>("Prometheus:Port");
            var server = new MetricServer(prometheusPort);
            server.Start();
            _processedJobCount.Inc();
        }

        private IConfiguration Configuration { get; }

        public void Init(CancellationToken token)
        {
            Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    _tickTock.Inc();
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }, token);

            _logger.LogInformation("The service [{ServiceName}] is successfully started at [{StartTime}] (UTC)",
                nameof(PrometheusService), DateTime.UtcNow.ToString("F"));
        }
    }
}