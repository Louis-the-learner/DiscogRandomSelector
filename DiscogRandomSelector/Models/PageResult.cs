using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace discogRandomSelector.Models
{
    public class PageResult
    {

        [JsonPropertyName("pagination")]
        public Pagination Pagination { get; set; }

        [JsonPropertyName("releases")]
        public List<Release> Releases  { get; set; }

    }
}