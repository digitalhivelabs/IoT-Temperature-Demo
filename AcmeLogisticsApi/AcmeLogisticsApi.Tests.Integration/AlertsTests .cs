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
            // Emulate that already exist.
            var response = await _client.PutAsJsonAsync("/api/telemetry/alerts/alert-12345/acknowledge", new { });

            response.IsSuccessStatusCode.Should().BeTrue();

            var alert = await response.Content.ReadFromJsonAsync<dynamic>();
            ((bool)alert!.Acknowledged).Should().BeTrue();
        }
    }
}
