using System.Text;
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
}
