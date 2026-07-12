using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Text.Json;

namespace AcmeLogisticsApi.Services
{
    public class DeviceService
    {
        private readonly RegistryManager _registryManager;

        public DeviceService(IConfiguration configuration)
        {
            var connectionString = configuration["IoTHub:ConnectionString"]
                ?? throw new InvalidOperationException("IoTHub connection string is missing.");

            _registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        }

        private static JsonElement ParseTwinCollection(string json)
        {
            return JsonDocument.Parse(json).RootElement.Clone();
        }

        public async Task<Twin> GetTwinAsync(string deviceId)
        {
            return await _registryManager.GetTwinAsync(deviceId);
        }

        public async Task<Twin> UpdateConfigAsync(string deviceId, JsonElement desiredProps)
        {
            var twin = await GetTwinAsync(deviceId);
            var patch = new Twin();
            patch.Properties.Desired = new TwinCollection(desiredProps.GetRawText());

            await _registryManager.UpdateTwinAsync(deviceId, patch, twin.ETag);
            return twin;
        }

        public async Task<double?> GetDesiredTemperatureThresholdAsync(string deviceId)
        {
            var twin = await GetTwinAsync(deviceId);
            var desiredJson = twin.Properties.Desired.ToJson();
            using var document = JsonDocument.Parse(desiredJson);
            var root = document.RootElement;

            if (!root.TryGetProperty("temperatureThresholdC", out var thresholdElement))
            {
                return null;
            }

            return thresholdElement.ValueKind switch
            {
                JsonValueKind.Number when thresholdElement.TryGetDouble(out var number) => number,
                JsonValueKind.String when double.TryParse(thresholdElement.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) => parsed,
                _ => null
            };
        }

        public async Task<object> GetStatusAsync(string deviceId)
        {
            var twin = await GetTwinAsync(deviceId);

            return new
            {
                deviceId,
                desired = ParseTwinCollection(twin.Properties.Desired.ToJson()),
                reported = ParseTwinCollection(twin.Properties.Reported.ToJson()),
                tags = ParseTwinCollection(twin.Tags.ToJson()),
                etag = twin.ETag
            };
        }
    }
}
