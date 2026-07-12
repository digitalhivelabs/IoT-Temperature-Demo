namespace AcmeLogisticsApi.Models
{
    public sealed class TelemetrySummary
    {
        public double? LastTemperature { get; set; }
        public int ReadsCount { get; set; }
        public int AlertsCount { get; set; }
    }
}
