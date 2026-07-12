using System.Text.Json.Serialization;

namespace AcmeLogisticsApi.Models
{
    public class AlertMessage : BaseMessage
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("acknowledged")]
        public bool Acknowledged { get; set; } = false;

        [JsonPropertyName("blobName")]
        public string BlobName { get; set; }
    }
}
