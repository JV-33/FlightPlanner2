using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace FlightPlanner.Models
{
    public class Airport
    {
        [Key]
        [JsonIgnore]
        public int ID { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        [JsonPropertyName("airport")]
        public string AirportCode { get; set; }
    }
}