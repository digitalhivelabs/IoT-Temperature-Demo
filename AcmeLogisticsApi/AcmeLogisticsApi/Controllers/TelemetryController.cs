using AcmeLogisticsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AcmeLogisticsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelemetryController : ControllerBase
    {
        private readonly TelemetryService _telemetryService;

        public TelemetryController(TelemetryService telemetryService)
        {
            _telemetryService = telemetryService;
        }

        [HttpGet("messages")]
        public async Task<IActionResult> GetLatestMessages()
        {
            var results = await _telemetryService.GetLatestMessagesAsync();
            return Ok(results);
        }

        [HttpGet("alerts")]
        public async Task<IActionResult> GetActiveAlerts()
        {
            var results = await _telemetryService.GetActiveAlertsAsync();
            return Ok(results);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var summary = await _telemetryService.GetSummaryAsync();
            return Ok(summary);
        }

        [HttpPut("alerts/{blobName}/acknowledge")]
        public async Task<IActionResult> AcknowledgeAlert(string blobName)
        {
            var alert = await _telemetryService.AcknowledgeAlertAsync(blobName);
            if (alert is null)
            {
                return NotFound();
            }

            return Ok(alert);
        }
    }
}
