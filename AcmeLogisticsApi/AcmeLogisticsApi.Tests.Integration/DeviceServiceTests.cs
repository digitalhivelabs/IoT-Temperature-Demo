using AcmeLogisticsApi.Services;
using FluentAssertions;
using Microsoft.Azure.Devices.Shared;
using System.Text.Json;

namespace AcmeLogisticsApi.Tests.Integration
{
    public class DeviceServiceTests
    {
        [Fact]
        public async Task GetDesiredTemperatureThresholdAsync_ReturnsNull_WhenDesiredPropertyMissing()
        {
            var twin = new Twin
            {
                Properties = { Desired = new TwinCollection("{}") }
            };

            var repository = new FakeTwinRepository(twin);
            var service = new DeviceService(repository);

            var threshold = await service.GetDesiredTemperatureThresholdAsync("device-1");

            threshold.Should().BeNull();
        }

        [Fact]
        public async Task GetDesiredTemperatureThresholdAsync_ReturnsValue_WhenDesiredPropertyExists()
        {
            var twin = new Twin
            {
                Properties = { Desired = new TwinCollection("{ \"temperatureThresholdC\": 5.0 }") }
            };

            var repository = new FakeTwinRepository(twin);
            var service = new DeviceService(repository);

            var threshold = await service.GetDesiredTemperatureThresholdAsync("device-1");

            threshold.Should().Be(5.0);
        }

        [Fact]
        public async Task GetStatusAsync_ReturnsTwinStatus_WithDesiredAndReportedElements()
        {
            var desiredJson = "{ \"temperatureThresholdC\": 5.0 }";
            var reportedJson = "{ \"someReported\": 123 }";
            var twin = new Twin
            {
                Properties = { Desired = new TwinCollection(desiredJson), Reported = new TwinCollection(reportedJson) }
            };

            var repository = new FakeTwinRepository(twin);
            var service = new DeviceService(repository);

            var status = await service.GetStatusAsync("device-1");

            status.Should().NotBeNull();
            var statusJson = JsonSerializer.Serialize(status);
            statusJson.Should().Contain("temperatureThresholdC");
            statusJson.Should().Contain("someReported");
        }

        private sealed class FakeTwinRepository : ITwinRepository
        {
            private readonly Twin _twin;

            public FakeTwinRepository(Twin twin)
            {
                _twin = twin;
            }

            public Task<Twin> GetTwinAsync(string deviceId)
            {
                return Task.FromResult(_twin);
            }

            public Task<Twin> UpdateTwinAsync(string deviceId, Twin patch, string etag)
            {
                throw new NotImplementedException();
            }
        }
    }
}
