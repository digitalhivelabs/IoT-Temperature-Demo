using AcmeLogisticsApi.Services;
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
        private readonly DeviceService _deviceService;

        public DeviceController(IConfiguration config, DeviceService deviceService)
        {
            var iotHubConnectionString = config["IoTHub:ConnectionString"]
                ?? throw new InvalidOperationException("IoTHub connection string is missing.");

            _serviceClient = ServiceClient.CreateFromConnectionString(iotHubConnectionString);
            _deviceService = deviceService;
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
            var twin = await _deviceService.UpdateConfigAsync(deviceId, desiredProps);
            return Ok(new { status = "Config updated", deviceId, desiredProps, etag = twin.ETag });
        }

        [HttpGet("{deviceId}/status")]
        public async Task<IActionResult> GetStatus(string deviceId)
        {
            var status = await _deviceService.GetStatusAsync(deviceId);
            return Ok(status);
        }
    }
}
