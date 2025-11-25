using System.Text.Json.Serialization;

namespace teniacoSample.Models
{
    public class RawDogApiResponse
    {
        [JsonPropertyName("message")]
        public Dictionary<string, List<string>> Message { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
