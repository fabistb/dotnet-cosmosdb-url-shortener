using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using UrlShortener.Configuration;

namespace UrlShortener.Models.Repositories
{
    public class CosmosDbRepository : ICosmosDbRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IOptions<CosmosDbOptions> _cosmosDbOptions;

        public CosmosDbRepository(CosmosClient cosmosClient, IOptions<CosmosDbOptions> cosmosDbOptions)
        {
            _cosmosClient = cosmosClient;
            _cosmosDbOptions = cosmosDbOptions;
        }

        public async Task<bool> CreateShortUrl(UrlInformation urlInformation)
        {
            try
            {
                var cosmosContainer = GetCosmosDbContainer();
                await cosmosContainer.CreateItemAsync(urlInformation, new PartitionKey(urlInformation.Id));
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                return false;
            }
        }
        
        public async Task<UrlInformation> GetShortUrl(string shortId)
        {
            try
            {
                var cosmosContainer = GetCosmosDbContainer();
                var urlItem = await cosmosContainer.ReadItemAsync<UrlInformation>(shortId, new PartitionKey(shortId));
                return urlItem;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }
        
        private Container GetCosmosDbContainer()
        {
            return _cosmosClient.GetContainer(_cosmosDbOptions.Value.CosmosNamespace,
                _cosmosDbOptions.Value.ShortUrlContainer);
        }
    }
}