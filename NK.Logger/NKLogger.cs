using Microsoft.Extensions.Logging;

namespace NK.Logger
{
    public class NKLogger : ILogger
    {
        private readonly Serilog.Core.Logger _logger;

        public NKLogger(Serilog.Core.Logger logger)
        {
            _logger = logger;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            return null!;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= LogLevel.Information;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            var message = formatter(state, exception!);
            if (exception != null)
            {
                message += Environment.NewLine + exception.ToString();
            }

            // Serilog にログを出力
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    _logger.Debug(message);
                    break;
                case LogLevel.Information:
                    _logger.Information(message);
                    break;
                case LogLevel.Warning:
                    _logger.Warning(message);
                    break;
                case LogLevel.Error:
                    _logger.Error(exception, message);
                    break;
                case LogLevel.Critical:
                    _logger.Fatal(exception, message);
                    break;
                default:
                    _logger.Information(message);
                    break;
            }
        }
    }
}
