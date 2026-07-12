namespace ColdChain.Agent;

public sealed class DeviceState
{
    public string HubDeviceId { get; private set; } = "shipment-001";
    public double TemperatureThresholdC { get; set; } = 8.0;
    public double LastTemperatureC { get; set; } = 5.0;
    public double LastHumidityPct { get; set; } = 55.0;
    public bool BuzzerActive { get; set; }
    public bool BuzzerSilencedByCloud { get; set; }
    public long SequenceNumber { get; private set; }
    public bool SimulateAlarmActive { get; set; }
    public int SimulateAlarmCyclesRemaining { get; set; }
    public DateTime? LastSentUtc { get; set; }
    public bool IsConnected { get; set; }

    private readonly object _sync = new();

    public void Configure(string hubDeviceId)
    {
        HubDeviceId = hubDeviceId;
    }

    public long NextSequence()
    {
        lock (_sync)
        {
            SequenceNumber++;
            return SequenceNumber;
        }
    }

    public void EvaluateBuzzer()
    {
        if (BuzzerSilencedByCloud)
        {
            BuzzerActive = false;
            return;
        }

        if (SimulateAlarmActive && SimulateAlarmCyclesRemaining > 0)
        {
            BuzzerActive = true;
            return;
        }

        BuzzerActive = LastTemperatureC > TemperatureThresholdC;
    }

    public void AfterTelemetrySent()
    {
        if (SimulateAlarmActive && SimulateAlarmCyclesRemaining > 0)
        {
            SimulateAlarmCyclesRemaining--;
            if (SimulateAlarmCyclesRemaining <= 0)
            {
                SimulateAlarmActive = false;
            }
        }
        else if (!SimulateAlarmActive && LastTemperatureC <= TemperatureThresholdC)
        {
            BuzzerActive = false;
        }

        if (BuzzerSilencedByCloud && LastTemperatureC <= TemperatureThresholdC && !SimulateAlarmActive)
        {
            BuzzerSilencedByCloud = false;
            Console.WriteLine("[state] Cloud silence expired because temperature returned to normal.");
        }
    }

    public TelemetryMessage BuildTelemetry()
    {
        EvaluateBuzzer();

        return new TelemetryMessage
        {
            DeviceId = "shipment-tracker-001",
            TemperatureC = Math.Round(LastTemperatureC, 1),
            HumidityPct = Math.Round(LastHumidityPct, 1),
            BuzzerActive = BuzzerActive,
            SequenceNumber = SequenceNumber,
            Timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "Z"
        };
    }

    public void PrintStatus()
    {
        Console.WriteLine($"Hub device id     : {HubDeviceId}");
        Console.WriteLine($"Payload device id : shipment-tracker-001");
        Console.WriteLine($"Temperature       : {LastTemperatureC:F1} C");
        Console.WriteLine($"Humidity          : {LastHumidityPct:F1} %");
        Console.WriteLine($"Threshold         : {TemperatureThresholdC:F1} C");
        Console.WriteLine($"Buzzer active     : {BuzzerActive}");
        Console.WriteLine($"Sequence number   : {SequenceNumber}");
        Console.WriteLine($"Connected         : {IsConnected}");
        Console.WriteLine($"Last sent         : {LastSentUtc?.ToString("u") ?? "(never)"}");
        Console.WriteLine($"Simulate alarm    : {SimulateAlarmActive} ({SimulateAlarmCyclesRemaining} cycles left)");
    }
}
