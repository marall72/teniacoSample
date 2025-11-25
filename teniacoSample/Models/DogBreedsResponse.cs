using System.Text.Json.Serialization;

namespace teniacoSample.Models
{
    public class DogBreedsResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("message")]
        public Dictionary<string, List<string>> Message { get; set; }
    }
}
