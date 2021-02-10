using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UrlShortener.Models;
using UrlShortener.Models.Repositories;
using UrlShortener.Utilities;

namespace UrlShortener.Services
{
    public class UrlShortenerService : IUrlShortenerService
    {
        private const string Characters = "abcdefghijklmnopqrstuvwxyz123456789";
        private const string Https = "https://";
        private const int ShortIdLength = 8;
        private readonly Clock _clock;

        private readonly ICosmosDbRepository _cosmosDbRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Random _random;

        public UrlShortenerService(ICosmosDbRepository cosmosDbRepository, Random random, Clock clock,
            IHttpContextAccessor httpContextAccessor)
        {
            _cosmosDbRepository = cosmosDbRepository;
            _random = random;
            _clock = clock;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ShortUrlResponse> CreateShortUrl(UrlToShorten urlToShorten)
        {
            if (!urlToShorten.LongUrl.Contains("https://") || !urlToShorten.LongUrl.Contains("http://"))
            {
                if (urlToShorten.LongUrl.StartsWith("www."))
                {
                    urlToShorten.LongUrl = $"{Https}{urlToShorten.LongUrl}";
                }
            }
            
            var validUri = Uri.IsWellFormedUriString(urlToShorten.LongUrl, UriKind.Absolute);

            if (validUri == false)
                throw new ArgumentException($"The submitted value isn't a valid url: {urlToShorten}");

            var shortId = CreateRandomString(ShortIdLength);

            var baseUrl = AssembleBaseUrl();

            var urlInformation = new UrlInformation
            {
                CreationDate = _clock.Now(),
                Id = shortId,
                ShortId = shortId,
                LogUrl = urlToShorten.LongUrl,
                ShortUrl = $"{baseUrl}/{shortId}"
            };

            var success = await _cosmosDbRepository.CreateShortUrl(urlInformation);

            if (success == true)
            {
                return new ShortUrlResponse
                {
                    ShortUrl = urlInformation.ShortUrl
                };
            }

            return null;
        }

        public async Task<string> GetShortUrl(string shortId)
        {
            var urlInformation = await _cosmosDbRepository.GetShortUrl(shortId);

            return urlInformation?.LogUrl;
        }

        private string AssembleBaseUrl()
        {
            var hostUrl = _httpContextAccessor.HttpContext.Request.Host;
            var httpContextScheme = _httpContextAccessor.HttpContext.Request.Scheme;

            var baseUrl = $"{httpContextScheme}:{hostUrl}";
            return baseUrl;
        }


        private string CreateRandomString(int lengthString)
        {
            var chars = new char[lengthString];

            for (var i = 0; i < lengthString; i++) chars[i] = Characters[_random.Next(Characters.Length)];

            return new string(chars);
        }
    }
}