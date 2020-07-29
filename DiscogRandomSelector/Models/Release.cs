using System;
using System.Text.Json.Serialization;

namespace discogRandomSelector.Models
{
    public class Release
    {

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("instance_id")]
        public int InstanceId { get; set; }

        [JsonPropertyName("date_added")]
        public DateTime DateAdded { get; set; }

        [JsonPropertyName("rating")]
        public int Rating { get; set; }

        [JsonPropertyName("basic_information")]
        public BasicInformation BasicInformation { get; set; }

    }

    public class BasicInformation
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("master_id")]
        public int MasterId { get; set; }

        [JsonPropertyName("master_url")]
        public Uri MasterUrl { get; set; }

        [JsonPropertyName("resource_url")]
        public Uri ResourceUrl { get; set; }

        [JsonPropertyName("thumb")]
        public String Thumb { get; set; }

        [JsonPropertyName("cover_image")]
        public String CoverImage { get; set; }

        [JsonPropertyName("title")]
        public String Title { get; set; }

        [JsonPropertyName("year")]
        public int Year { get; set; }

        [JsonPropertyName("formats")]
        public Formats[] Formats { get; set; }

        [JsonPropertyName("labels")]
        public Labels[] Labels { get; set; }

        [JsonPropertyName("artists")]
        public Artists[] Artists { get; set; }

        [JsonPropertyName("genres")]
        public String[] Genres { get; set; }

        [JsonPropertyName("styles")]
        public String[] Styles { get; set; }

    }

    public class Formats 
    {
        [JsonPropertyName("name")]
        public String Name { get; set; }

        [JsonPropertyName("qty")]
        public String Qty { get; set; }

        [JsonPropertyName("descriptions")]
        public String[] Descriptions { get; set; }
    }

    public class Labels
    {
        [JsonPropertyName("name")]
        public String Name { get; set; }

        [JsonPropertyName("catno")]
        public String CatNo { get; set; }

        [JsonPropertyName("entity_type")]
        public String EntityType { get; set; }

        [JsonPropertyName("entity_type_name")]
        public String EntityTypeName { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("resource_url")]
        public Uri ResourceUrl { get; set; }
    }

    public class Artists
    {
        [JsonPropertyName("name")]
        public String Name { get; set; }

        [JsonPropertyName("anv")]
        public String Anv { get; set; }

        [JsonPropertyName("join")]
        public String Join { get; set; }

        [JsonPropertyName("role")]
        public String Role { get; set; }

        [JsonPropertyName("tracks")]
        public String Tracks  { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("resource_url")]
        public Uri ResourceUrl { get; set; }
    }


}