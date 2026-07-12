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

        // Alerts (needs to be refactored and no hard coded values.)
        private int consecutiveHighReadings = 0;
        private readonly int _sustainedHighReadingsThreshold;
        private readonly string _monitoredDeviceId;
        private readonly double _defaultTemperatureThresholdC;
        private readonly string _alertMessage;
        private readonly DeviceService _deviceService;

        public TelemetryIngestService(
            string eventHubConnectionString,
            string eventHubName,
            string blobConnectionString,
            int sustainedHighReadingsThreshold,
            string monitoredDeviceId,
            double defaultTemperatureThresholdC,
            string alertMessage,
            DeviceService deviceService)
        {
            _sustainedHighReadingsThreshold = sustainedHighReadingsThreshold;
            _monitoredDeviceId = monitoredDeviceId;
            _defaultTemperatureThresholdC = defaultTemperatureThresholdC;
            _alertMessage = alertMessage;
            _deviceService = deviceService;

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
                var threshold = await _deviceService.GetDesiredTemperatureThresholdAsync(_monitoredDeviceId)
                    ?? _defaultTemperatureThresholdC;

                if (telemetry?.TemperatureC > threshold)
                {
                    consecutiveHighReadings++;
                }
                else
                {
                    consecutiveHighReadings = 0;
                }

                if (consecutiveHighReadings >= _sustainedHighReadingsThreshold)
                {
                    blobName = $"alert-{DateTime.UtcNow:yyyyMMddHHmmssfff}.json";

                    var alert = new AlertMessage
                    {
                        DeviceId = telemetry?.DeviceId,
                        TemperatureC = telemetry.TemperatureC,
                        Timestamp = telemetry.Timestamp,
                        TemperatureThreshold = threshold,
                        Message = _alertMessage,
                        Acknowledged = false,
                        BlobName = blobName
                    };

                    // Guardar alerta en Blob Storage o DB
                    blobClient = _telemetryContainer.GetBlobClient(blobName);
                    using var streamAlert = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(alert)));
                    await blobClient.UploadAsync(streamAlert, overwrite: true);

                    // Reinicia contador para evitar múltiples alertas seguidas
                    consecutiveHighReadings = 0;
                }
                #endregion Alert


                // Saves to a txt File (testing before adding Blob storage)
                //NOTE: if this change to a file considere that get data from files in the controllers must change.
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
