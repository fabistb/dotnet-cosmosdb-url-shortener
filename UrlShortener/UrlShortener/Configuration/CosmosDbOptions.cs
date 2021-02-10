namespace UrlShortener.Configuration
{
    public class CosmosDbOptions
    {
        public const string ConfigPath = "CosmosDb";

        public string CosmosNamespace = "urlshortener";

        public string ShortUrlContainer = "shorturl";
        
        public string CosmosDbConnectionString { get; set; }
    }
}