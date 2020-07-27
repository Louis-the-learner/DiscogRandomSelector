using System;
using System.Text.Json.Serialization;

namespace discogSelector.Models
{
    public class Pagination
    {

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("pages")]
        public int Pages { get; set; }

        [JsonPropertyName("per_page")]
        public int PerPage { get; set; }

        [JsonPropertyName("items")]
        public int Items { get; set ;}

        [JsonPropertyName("urls")]
        public Urls Urls { get; set; }

    }

    public class Urls
    {
        [JsonPropertyName("first")]
        public Uri First { get; set; }

        [JsonPropertyName("last")]
        public Uri Last { get; set; }

        [JsonPropertyName("prev")]
        public Uri Prev { get; set; }

        [JsonPropertyName("next")]
        public Uri Next { get; set; }

    }
}