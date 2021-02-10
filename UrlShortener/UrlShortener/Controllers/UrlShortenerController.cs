using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UrlShortener.Models;
using UrlShortener.Services;

namespace UrlShortener.Controllers
{
    
    [ApiController]
    [Route("")]
    public class UrlShortenerController : BaseController
    {
        private readonly IUrlShortenerService _urlShortenerService;

        public UrlShortenerController(ILogger<BaseController> logger, IUrlShortenerService urlShortenerService) : base(logger)
        {
            _urlShortenerService = urlShortenerService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateShortUrl([FromBody] UrlToShorten urlToShorten)
        {
            
            var shortUrl = await _urlShortenerService.CreateShortUrl(urlToShorten);

            if (shortUrl == null)
            {
                return BadRequest();
            }

            return Ok(shortUrl);
        }

        [HttpGet("{shortId}")]
        public async Task<IActionResult> RedirectToUrl([FromRoute] string shortId)
        {
            var longUrl = await _urlShortenerService.GetShortUrl(shortId);
            if (longUrl == null)
            {
                return NotFound();
            }

            return Redirect(longUrl);
        }
    }
}