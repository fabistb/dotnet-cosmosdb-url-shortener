using System;
using Newtonsoft.Json;

namespace UrlShortener.Models
{
    public class UrlInformation
    {
        [JsonProperty("id")] 
        public string Id { get; set; }

        public string ShortId { get; set; }

        public string LogUrl { get; set; }

        public string ShortUrl { get; set; }

        public DateTime CreationDate { get; set; }
    }
}