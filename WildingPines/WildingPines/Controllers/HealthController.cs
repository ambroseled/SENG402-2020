using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WildingPines.Models;

namespace WildingPines.Controllers
{
    /// <summary>
    /// Used to check that the server is running and reachable.
    /// </summary>
    [Route("api/v1/health")]
    public class HealthController : Controller
    {
        private readonly ILogger _logger;

        public HealthController(ILogger<HealthController> iLogger)
        {
            _logger = iLogger;
        }

        [HttpGet]
        public HealthCheck GetHealthCheck()
        {
            _logger.Log(LogLevel.Information, $"Responded with health check on {DateTime.Now}");
            return new HealthCheck();
        }
    }
}