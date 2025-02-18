using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NK.Logger.AzureQueue
{
    public class AzureQueueLoggerProvider : ILoggerProvider
    {
        private readonly string _queueName;
        private readonly QueueClient _queueClient;
        private bool _disposed = false;
        public AzureQueueLoggerProvider(string connectionString, string queueName)
        {
            _queueName = queueName;
            _queueClient = new QueueClient(connectionString, _queueName);
            _queueClient.CreateIfNotExists();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new AzureQueueLogger(_queueClient);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }

    public class AzureQueueLogger : ILogger
    {
        private readonly QueueClient _queueClient;

        public AzureQueueLogger(QueueClient queueClient)
        {
            _queueClient = queueClient;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => null!;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            var logMessage = new
            {
                Level = logLevel.ToString(),
                Message = formatter(state, exception),
                Exception = exception?.ToString(),
                Timestamp = DateTime.UtcNow
            };

            var messageJson = JsonSerializer.Serialize(logMessage);
            _queueClient.SendMessage(Convert.ToBase64String(Encoding.UTF8.GetBytes(messageJson)));
        }
    }
}
