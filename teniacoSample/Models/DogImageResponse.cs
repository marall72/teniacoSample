using System.Text.Json.Serialization;

namespace teniacoSample.Models
{
    public class DogImageResponse
    {
        [JsonPropertyName("message")]
        public string ImageUrl { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
