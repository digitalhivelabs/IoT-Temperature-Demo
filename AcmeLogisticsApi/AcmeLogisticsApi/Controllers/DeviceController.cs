using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using System.Text;
using System.Text.Json;

namespace AcmeLogisticsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly ServiceClient _serviceClient;
        private readonly string _iotHubConnectionString;

        public DeviceController(IConfiguration config)
        {
            _iotHubConnectionString = config["IoTHub:ConnectionString"];
            _serviceClient = ServiceClient.CreateFromConnectionString(_iotHubConnectionString);
        }

        [HttpPost("{deviceId}/command")]
        public async Task<IActionResult> SendCommand(string deviceId, [FromBody] object command)
        {
            string jsonCommand = JsonSerializer.Serialize(command);
            var message = new Message(Encoding.UTF8.GetBytes(jsonCommand));

            await _serviceClient.SendAsync(deviceId, message);

            return Ok(new { status = "Command sent", deviceId, command });
        }

        [HttpPut("{deviceId}/config")]
        public async Task<IActionResult> UpdateConfig(string deviceId, [FromBody] JsonElement desiredProps)
        {
            var registryManager = RegistryManager.CreateFromConnectionString(_iotHubConnectionString);

            var twin = await registryManager.GetTwinAsync(deviceId);

            var patch = new Twin();
            patch.Properties.Desired = new TwinCollection(desiredProps.GetRawText());

            await registryManager.UpdateTwinAsync(deviceId, patch, twin.ETag);

            return Ok(new { status = "Config updated", deviceId, desiredProps });
        }

        [HttpGet("{deviceId}/status")]
        public async Task<IActionResult> GetStatus(string deviceId)
        {
            var registryManager = RegistryManager.CreateFromConnectionString(_iotHubConnectionString);
            var twin = await registryManager.GetTwinAsync(deviceId);

            return Ok(new
            {
                deviceId,
                desired = JsonDocument.Parse(twin.Properties.Desired.ToJson()).RootElement,
                reported = JsonDocument.Parse(twin.Properties.Reported.ToJson()).RootElement,
                tags = JsonDocument.Parse(twin.Tags.ToJson()).RootElement,
                etag = twin.ETag
            });
        }
    }
}
