using System.Threading.Tasks;
using UrlShortener.Models;

namespace UrlShortener.Services
{
    public interface IUrlShortenerService
    {
        Task<ShortUrlResponse> CreateShortUrl(UrlToShorten urlToShorten);

        Task<string> GetShortUrl(string shortId);
    }
}