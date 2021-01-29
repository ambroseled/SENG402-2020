using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WildingPines.Models.Responses;

namespace WildingPines.Controllers
{
    /// <summary>
    /// Controls the lifecycle of the whole system.
    /// </summary>
    [Route("api/v1")]
    public class LifecycleController : Controller
    {
        private readonly ILogger _logger;

        public LifecycleController(ILogger<LifecycleController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Shuts down the NUC when asked to.
        /// NOTE: only works on Linux (tested on Ubuntu) and requires admin
        /// privileges to run the shutdown command without password or sudo.
        /// </summary>
        /// <returns></returns>
        [Route("shutdown")]
        [HttpPost]
        public ActionResult<SuccessResponse> Shutdown()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "shutdown",
                    Arguments = "-h now",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                }
            };
            process.Start();
            using var reader = process.StandardOutput;
            var result = reader.ReadToEnd();
            _logger.Log(LogLevel.Information, "Requested shutdown from OS");
            _logger.Log(LogLevel.Information, result);
            return new SuccessResponse("Requested shutdown from OS");
        }
    }
}