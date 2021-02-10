using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace UrlShortener.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected readonly ILogger<BaseController> _logger;

        protected BaseController(ILogger<BaseController> logger)
        {
            _logger = logger;
        }
    }
}