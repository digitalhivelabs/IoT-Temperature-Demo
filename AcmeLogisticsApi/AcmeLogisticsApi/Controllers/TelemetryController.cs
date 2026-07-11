using AcmeLogisticsApi.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AcmeLogisticsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelemetryController : ControllerBase
    {
        private readonly BlobContainerClient _telemetryContainer;
        private readonly BlobContainerClient _alertsContainer;

        public TelemetryController(IConfiguration config)
        {
            string blobConnectionString = config["EventHub:BlobStorage"];

            _telemetryContainer = new BlobContainerClient(blobConnectionString, "telemetry");
            _alertsContainer = new BlobContainerClient(blobConnectionString, "telemetry"); // mismo contenedor si guardas alertas ahí
        }

        [HttpGet("messages")]
        public async Task<IActionResult> GetLatestMessages()
        {
            var blobs = _telemetryContainer.GetBlobs()
                                           .OrderByDescending(b => b.Properties.CreatedOn)
                                           .Take(10);

            var results = new List<object>();

            foreach (var blob in blobs)
            {
                var client = _telemetryContainer.GetBlobClient(blob.Name);
                var content = await client.DownloadContentAsync();
                results.Add(JsonSerializer.Deserialize<TelemetryMessage>(content.Value.Content.ToString()));
            }

            return Ok(results);
        }

        [HttpGet("alerts")]
        public async Task<IActionResult> GetActiveAlerts()
        {
            var blobs = _alertsContainer.GetBlobs()
                                        .Where(b => b.Name.StartsWith("alert-"))
                                        .OrderByDescending(b => b.Properties.CreatedOn)
                                        .Take(10);

            var results = new List<object>();

            foreach (var blob in blobs)
            {
                var client = _alertsContainer.GetBlobClient(blob.Name);
                var content = await client.DownloadContentAsync();
                results.Add(JsonSerializer.Deserialize<AlertMessage>(content.Value.Content.ToString()));
            }

            return Ok(results);
        }

        [HttpPut("alerts/{blobName}/acknowledge")]
        public async Task<IActionResult> AcknowledgeAlert(string blobName)
        {
            var blobClient = _alertsContainer.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
                return NotFound();

            var content = await blobClient.DownloadContentAsync();
            var alert = JsonSerializer.Deserialize<AlertMessage>(content.Value.Content.ToString());

            alert.Acknowledged = true;

            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(alert)));
            await blobClient.UploadAsync(stream, overwrite: true);

            return Ok(alert);
        }

    }
}
