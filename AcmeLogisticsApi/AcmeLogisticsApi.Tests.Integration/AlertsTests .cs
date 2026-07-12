using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;

namespace AcmeLogisticsApi.Tests.Integration
{
    public class AlertsTests : IntegrationTestBase
    {
        public AlertsTests(WebApplicationFactory<Program> factory) : base(factory) { }

        [Fact]
        public async Task OperatorCanAcknowledgeAlert()
        {
            // Arrange
            var blobConnectionString = Configuration["EventHub:BlobStorage"];
            var blobClient = new Azure.Storage.Blobs.BlobContainerClient(blobConnectionString, "telemetry");
            await blobClient.CreateIfNotExistsAsync();

            var alert = new AcmeLogisticsApi.Models.AlertMessage
            {
                DeviceId = "temperature-pharma-001",
                TemperatureC = 12.5,
                Timestamp = DateTime.UtcNow,
                Message = "Test alert",
                Acknowledged = false,
                BlobName = "alert-12345.json"
            };

            var blob = blobClient.GetBlobClient(alert.BlobName);
            await using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(alert))))
            {
                await blob.UploadAsync(stream, overwrite: true);
            }

            // Act
            var response = await _client.PutAsJsonAsync($"/api/telemetry/alerts/{alert.BlobName}/acknowledge", new { });

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            var acknowledgedAlert = await response.Content.ReadFromJsonAsync<AcmeLogisticsApi.Models.AlertMessage>();
            acknowledgedAlert.Should().NotBeNull();
            acknowledgedAlert!.Acknowledged.Should().BeTrue();
        }
    }
}
