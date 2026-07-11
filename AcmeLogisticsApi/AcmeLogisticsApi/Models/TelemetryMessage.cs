using System.Text.Json.Serialization;

namespace AcmeLogisticsApi.Models
{
    public class TelemetryMessage
    {
        [JsonPropertyName("deviceId")]
        public string DeviceId { get; set; }

        [JsonPropertyName("temperatureC")]
        public double TemperatureC { get; set; }

        [JsonPropertyName("humidityPct")]
        public double HumidityPct { get; set; }

        [JsonPropertyName("buzzerActive")]
        public bool BuzzerActive { get; set; }

        [JsonPropertyName("sequenceNumber")]
        public int SequenceNumber { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
