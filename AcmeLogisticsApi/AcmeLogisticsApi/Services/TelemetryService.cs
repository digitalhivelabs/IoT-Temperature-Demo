using AcmeLogisticsApi.Models;
using Azure.Storage.Blobs;
using System.Text.Json;

namespace AcmeLogisticsApi.Services
{
    public class TelemetryService
    {
        private readonly BlobContainerClient _telemetryContainer;
        private readonly BlobContainerClient _alertsContainer;

        public TelemetryService(IConfiguration configuration)
        {
            string blobConnectionString = configuration["EventHub:BlobStorage"]
                ?? throw new InvalidOperationException("EventHub blob storage connection string is missing.");

            _telemetryContainer = new BlobContainerClient(blobConnectionString, "telemetry");
            _telemetryContainer.CreateIfNotExists();

            _alertsContainer = new BlobContainerClient(blobConnectionString, "telemetry");
            _alertsContainer.CreateIfNotExists();
        }

        public async Task<IEnumerable<TelemetryMessage>> GetLatestMessagesAsync(int maxResults = 10)
        {
            var blobs = _telemetryContainer.GetBlobs()
                                           .OrderByDescending(b => b.Properties.CreatedOn)
                                           .Take(maxResults);

            var results = new List<TelemetryMessage>();

            foreach (var blob in blobs)
            {
                var client = _telemetryContainer.GetBlobClient(blob.Name);
                var content = await client.DownloadContentAsync();
                var telemetry = JsonSerializer.Deserialize<TelemetryMessage>(content.Value.Content.ToString());

                if (telemetry is not null)
                {
                    results.Add(telemetry);
                }
            }

            return results;
        }

        public async Task<IEnumerable<AlertMessage>> GetActiveAlertsAsync(int maxResults = 10)
        {
            var blobs = _alertsContainer.GetBlobs()
                                        .Where(b => b.Name.StartsWith("alert-"))
                                        .OrderByDescending(b => b.Properties.CreatedOn)
                                        .Take(maxResults);

            var results = new List<AlertMessage>();

            foreach (var blob in blobs)
            {
                var client = _alertsContainer.GetBlobClient(blob.Name);
                var content = await client.DownloadContentAsync();
                var alert = JsonSerializer.Deserialize<AlertMessage>(content.Value.Content.ToString());

                if (alert is not null)
                {
                    results.Add(alert);
                }
            }

            return results;
        }

        public async Task<AlertMessage?> AcknowledgeAlertAsync(string blobName)
        {
            var blobClient = _alertsContainer.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
            {
                return null;
            }

            var content = await blobClient.DownloadContentAsync();
            var alert = JsonSerializer.Deserialize<AlertMessage>(content.Value.Content.ToString());

            if (alert is null)
            {
                return null;
            }

            alert.Acknowledged = true;

            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(alert)));
            await blobClient.UploadAsync(stream, overwrite: true);

            return alert;
        }
    }
}
