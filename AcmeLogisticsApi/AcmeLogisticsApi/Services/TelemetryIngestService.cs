using AcmeLogisticsApi.Models;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Storage.Blobs;
using System.Text.Json;

namespace AcmeLogisticsApi.Services
{
    public class TelemetryIngestService
    {
        private readonly EventProcessorClient _processor;
        private readonly BlobContainerClient _telemetryContainer;

        public TelemetryIngestService(string eventHubConnectionString, string eventHubName, string blobConnectionString)
        {
            var blobClient = new BlobContainerClient(blobConnectionString, "checkpoints");

            // Contenedor para telemetría
            _telemetryContainer = new BlobContainerClient(blobConnectionString, "telemetry");
            _telemetryContainer.CreateIfNotExists();

            _processor = new EventProcessorClient(blobClient, EventHubConsumerClient.DefaultConsumerGroupName, eventHubConnectionString, eventHubName);


            _processor.ProcessEventAsync += async (args) =>
            {
                string body = args.Data.EventBody.ToString();
                var telemetry = JsonSerializer.Deserialize<TelemetryMessage>(body);

                // Saves to a txt File (testing before adding Blob storage)
                //await File.AppendAllTextAsync("telemetry.log", body + Environment.NewLine);
                
                // Saving the data to Blob Storage Telemtry Container
                string blobName = $"telemetry-{DateTime.UtcNow:yyyyMMddHHmmssfff}.json";
                BlobClient blobClient = _telemetryContainer.GetBlobClient(blobName);

                using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(body));
                await blobClient.UploadAsync(stream, overwrite: true);

                await args.UpdateCheckpointAsync(args.CancellationToken);
            };

            _processor.ProcessErrorAsync += async (args) =>
            {
                Console.WriteLine($"Error: {args.Exception.Message}");
            };
        }

        public async Task StartAsync()
        {
            await _processor.StartProcessingAsync();
        }

        public async Task StopAsync()
        {
            await _processor.StopProcessingAsync();
        }
    }
}
