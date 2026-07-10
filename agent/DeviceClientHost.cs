using System.Text;
using System.Text.Json;
using Microsoft.Azure.Devices.Client;

namespace ColdChain.Agent;

public sealed class DeviceClientHost : IAsyncDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

    private readonly DeviceState _state;
    private readonly DownlinkHandler _downlink;
    private readonly string _connectionString;
    private readonly List<TelemetryMessage> _offlineBuffer = new();
    private DeviceClient? _client;

    public DeviceClientHost(DeviceState state, DownlinkHandler downlink, string connectionString)
    {
        _state = state;
        _downlink = downlink;
        _connectionString = connectionString;
    }

    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        _client = DeviceClient.CreateFromConnectionString(_connectionString, TransportType.Mqtt);
        await _client.OpenAsync(cancellationToken);

        await _client.SetReceiveMessageHandlerAsync(_downlink.HandleCloudToDeviceMessageAsync, null);
        await _downlink.InitializeTwinAtStartupAsync(_client, cancellationToken);

        _state.IsConnected = true;
        Console.WriteLine($"Connected to IoT Hub as '{_state.HubDeviceId}'.");

        await FlushOfflineBufferAsync(cancellationToken);
    }

    public async Task SendTelemetryAsync(bool advanceSequence, CancellationToken cancellationToken)
    {
        if (_client is null)
        {
            throw new InvalidOperationException("Device is not connected. Run the connect command first.");
        }

        if (advanceSequence)
        {
            _ = _state.NextSequence();
        }

        var telemetry = _state.BuildTelemetry();
        await TrySendAsync(telemetry, cancellationToken);
        _state.AfterTelemetrySent();
    }

    public async Task RunTelemetryLoopAsync(TimeSpan interval, CancellationToken cancellationToken)
    {
        await ConnectAsync(cancellationToken);

        using var timer = new PeriodicTimer(interval);
        await SendTelemetryAsync(advanceSequence: true, cancellationToken);

        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            if (!_state.SimulateAlarmActive)
            {
                _state.LastTemperatureC = 4.5 + Random.Shared.NextDouble() * 2.5;
                _state.LastHumidityPct = 58 + Random.Shared.NextDouble() * 8;
            }

            await SendTelemetryAsync(advanceSequence: true, cancellationToken);
        }
    }

    public async Task SimulateAlarmAsync(int cycles, CancellationToken cancellationToken)
    {
        if (_client is null)
        {
            await ConnectAsync(cancellationToken);
        }

        _state.SimulateAlarmActive = true;
        _state.SimulateAlarmCyclesRemaining = cycles;

        for (var i = 0; i < cycles; i++)
        {
            _state.LastTemperatureC = 12 + Random.Shared.NextDouble() * 3;
            _state.LastHumidityPct = 70 + Random.Shared.NextDouble() * 5;
            await SendTelemetryAsync(advanceSequence: true, cancellationToken);
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }
    }

    private async Task TrySendAsync(TelemetryMessage telemetry, CancellationToken cancellationToken)
    {
        if (_client is null)
        {
            _offlineBuffer.Add(telemetry);
            return;
        }

        try
        {
            var payload = JsonSerializer.Serialize(telemetry, JsonOptions);
            using var message = new Message(Encoding.UTF8.GetBytes(payload));
            message.ContentType = "application/json";
            message.ContentEncoding = "utf-8";

            await _client.SendEventAsync(message, cancellationToken);
            _state.LastSentUtc = DateTime.UtcNow;
            Console.WriteLine($"[uplink] seq={telemetry.SequenceNumber} temp={telemetry.TemperatureC}C buzzer={telemetry.BuzzerActive}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[uplink] Send failed ({ex.Message}). Buffering message seq={telemetry.SequenceNumber}.");
            _offlineBuffer.Add(telemetry);
            _state.IsConnected = false;
        }
    }

    private async Task FlushOfflineBufferAsync(CancellationToken cancellationToken)
    {
        if (_offlineBuffer.Count == 0)
        {
            return;
        }

        Console.WriteLine($"[uplink] Replaying {_offlineBuffer.Count} buffered message(s)...");
        var pending = _offlineBuffer.ToList();
        _offlineBuffer.Clear();

        foreach (var telemetry in pending)
        {
            await TrySendAsync(telemetry, cancellationToken);
        }

        _state.IsConnected = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_client is not null)
        {
            await _client.CloseAsync();
            _client.Dispose();
        }
    }
}
