using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UrlShortener.Configuration;

namespace UrlShortener.Utilities
{
    public static class CosmosDbServiceExtension
    {
        public static IServiceCollection AddCosmosDb(this IServiceCollection service, IConfiguration configuration)
        {
            configuration["CosmosDb:CosmosDbConnectionString"] = configuration["cosmos-connectionstring"];
            service.Configure<CosmosDbOptions>(configuration.GetSection("CosmosDb"));
            service.AddSingleton(serviceProvider =>
                new CosmosClient(
                    serviceProvider.GetService<IOptions<CosmosDbOptions>>()?.Value.CosmosDbConnectionString));
            return service;
        }
    }
}