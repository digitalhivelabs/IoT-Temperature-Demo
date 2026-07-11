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

        //Alerts (needs to be refactores and no hard coded values.)
        private int consecutiveHighReadings = 0;
        private const double TemperatureThreshold = 8.0;

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


                string blobName = string.Empty;
                BlobClient blobClient;

                #region Alert (needs to be refactored)
                if (telemetry?.TemperatureC > TemperatureThreshold)
                {
                    consecutiveHighReadings++;
                }
                else
                {
                    consecutiveHighReadings = 0;
                }

                if (consecutiveHighReadings >= 3)
                {
                    var alert = new
                    {
                        deviceId = telemetry?.DeviceId,
                        temperature = telemetry?.TemperatureC,
                        timestamp = telemetry?.Timestamp,
                        message = "Alert sustained: temperature exceeds"
                    };

                    // Guardar alerta en Blob Storage o DB
                    blobName = $"alert-{DateTime.UtcNow:yyyyMMddHHmmssfff}.json";
                    blobClient = _telemetryContainer.GetBlobClient(blobName);
                    using var streamAlert = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(alert)));
                    await blobClient.UploadAsync(streamAlert, overwrite: true);

                    // Reinicia contador para evitar múltiples alertas seguidas
                    consecutiveHighReadings = 0;
                }
                #endregion Alert


                // Saves to a txt File (testing before adding Blob storage)
                //await File.AppendAllTextAsync("telemetry.log", body + Environment.NewLine);

                // Saving the data to Blob Storage Telemtry Container
                blobName = $"telemetry-{DateTime.UtcNow:yyyyMMddHHmmssfff}.json";
                blobClient = _telemetryContainer.GetBlobClient(blobName);

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
