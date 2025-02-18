using Microsoft.Extensions.Logging;
using Serilog;

using Azure.Storage.Blobs;

namespace NK.Logger.AzureBlob
{
    public class AzureBlobLoggerProvider : ILoggerProvider
    {
        private readonly Serilog.Core.Logger _logger;
        public AzureBlobLoggerProvider(string connectionString, string blobContainerName, string logFileName = null!)
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            var blobContinerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
            blobContinerClient.CreateIfNotExists();
            logFileName = logFileName ?? $"logs/{DateTime.UtcNow:yyyy-MM-dd}.log";

            _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.AzureBlobStorage(
                    connectionString: connectionString,
                    storageFileName: logFileName,
                    storageContainerName: blobContainerName)
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
