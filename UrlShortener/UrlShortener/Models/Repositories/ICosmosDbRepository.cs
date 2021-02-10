using System.Threading.Tasks;

namespace UrlShortener.Models.Repositories
{
    public interface ICosmosDbRepository 
    {
        Task<bool> CreateShortUrl(UrlInformation urlInformation);

        Task<UrlInformation> GetShortUrl(string shortId);
        
    }
}