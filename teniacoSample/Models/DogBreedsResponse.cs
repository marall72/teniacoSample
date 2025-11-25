using System.Text.Json.Serialization;

namespace teniacoSample.Models
{
    public class DogBreedsResponse
    {
        public string Status { get; set; }
        public List<Breed> Breeds { get; set; }
    }
}
