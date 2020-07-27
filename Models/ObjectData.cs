using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace discogSelector.Models
{
    public class ObjectData
    {

        [JsonPropertyName("pagination")]
        public Pagination Pagination { get; set; }

        [JsonPropertyName("releases")]
        public List<Release> Releases  { get; set; }

    }
}