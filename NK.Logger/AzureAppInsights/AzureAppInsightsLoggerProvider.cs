using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;
using NK.Logger.AzureBlob;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NK.Logger.AzureAppInsights
{
    public class AzureAppInsightsLoggerProvider : ILoggerProvider
    {
        private readonly Serilog.Core.Logger _logger;

        public AzureAppInsightsLoggerProvider(string connectionString, string instrumentationKey,string logFileName)
        {
            var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
            telemetryConfiguration.ConnectionString = connectionString;

            logFileName = logFileName ?? $"logs-{DateTime.UtcNow:yyyy-MM-dd}.log";

            _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(logFileName, rollingInterval: RollingInterval.Day)
                .WriteTo.ApplicationInsights(
                    telemetryConfiguration,
                    TelemetryConverter.Traces) 
                .CreateLogger();
        }

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
        {
            return new NKLogger(_logger);
        }

        public void Dispose()
        {
            _logger.Dispose();
        }
    }
}
