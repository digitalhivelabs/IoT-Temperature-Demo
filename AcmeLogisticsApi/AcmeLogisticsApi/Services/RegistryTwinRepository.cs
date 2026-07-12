using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;

namespace AcmeLogisticsApi.Services
{
    public class RegistryTwinRepository : ITwinRepository
    {
        private readonly RegistryManager _registryManager;

        public RegistryTwinRepository(IConfiguration configuration)
        {
            var connectionString = configuration["IoTHub:ConnectionString"]
                ?? throw new InvalidOperationException("IoTHub connection string is missing.");

            _registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        }

        public async Task<Twin> GetTwinAsync(string deviceId)
        {
            return await _registryManager.GetTwinAsync(deviceId);
        }

        public async Task<Twin> UpdateTwinAsync(string deviceId, Twin patch, string etag)
        {
            return await _registryManager.UpdateTwinAsync(deviceId, patch, etag);
        }
    }
}
