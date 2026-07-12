using System.Text;
using System.Text.Json;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;

namespace ColdChain.Agent;

public sealed class DownlinkHandler
{
    private readonly DeviceState _state;

    public DownlinkHandler(DeviceState state)
    {
        _state = state;
    }

    public async Task InitializeTwinAtStartupAsync(DeviceClient client, CancellationToken cancellationToken)
    {
        var twin = await client.GetTwinAsync(cancellationToken);
        ApplyDesiredProperties(twin.Properties.Desired);
    }

    public Task<MessageResponse> HandleCloudToDeviceMessageAsync(Message message, object _)
    {
        var body = Encoding.UTF8.GetString(message.GetBytes());
        Console.WriteLine($"[downlink] Cloud-to-device message received: {body}");

        try
        {
            var payload = JsonSerializer.Deserialize<CloudCommand>(body);
            if (payload?.Command is not null)
            {
                switch (payload.Command)
                {
                    case "silence_buzzer":
                        _state.BuzzerSilencedByCloud = true;
                        _state.BuzzerActive = false;
                        Console.WriteLine("[downlink] Buzzer silenced by cloud command.");
                        break;
                    case "activate_buzzer":
                        _state.SimulateAlarmActive = true;
                        _state.SimulateAlarmCyclesRemaining = 5;
                        _state.BuzzerSilencedByCloud = false;
                        _state.BuzzerActive = true;
                        Console.WriteLine("[downlink] Buzzer activated by cloud command.");
                        break;
                    default:
                        Console.WriteLine($"[downlink] Unknown cloud command: {payload.Command}");
                        break;
                }
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[downlink] Failed to parse command payload: {ex.Message}");
        }

        return Task.FromResult(MessageResponse.Completed);
    }

    private void ApplyDesiredProperties(TwinCollection desired)
    {
        if (desired.Contains("temperatureThresholdC"))
        {
            var threshold = (double)desired["temperatureThresholdC"];
            Console.WriteLine($"[twin] Desired temperatureThresholdC={threshold} (applied at startup only)");
            _state.TemperatureThresholdC = threshold;
        }
    }

    private sealed class CloudCommand
    {
        public string? Command { get; set; }
        public string? CorrelationId { get; set; }
    }
}
