using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WildingPines.Communication.SprayModuleIpc;
using WildingPines.Controllers.Responses;
using WildingPines.Models;
using WildingPines.Models.Responses;

namespace WildingPines.Controllers
{
    /// <summary>
    /// Controls the nozzles in the spray boom.
    /// </summary>
    [Route("api/v1/nozzles")]
    public class NozzlesController : Controller
    {
        private readonly ILogger<NozzlesController> _logger;
        private readonly ISprayModuleBridge _sprayModuleBridge;
        private readonly List<Nozzle> mockNozzles = new List<Nozzle>
        {
            new Nozzle(0, false, "Nozzle 0"),
            new Nozzle(0, false, "Nozzle 1"),
            new Nozzle(0, true, "Nozzle 2"),
            new Nozzle(0, false, "Nozzle 3")
        };

        public NozzlesController(ILogger<NozzlesController> logger, ISprayModuleBridge sprayModuleBridge)
        {
            _logger = logger;
            _sprayModuleBridge = sprayModuleBridge;
        }

        /// <returns>
        /// The available spray nozzles in the system.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<List<Nozzle>>> GetAvailableNozzles()
        {
            try
            {
                var availableNozzles = await _sprayModuleBridge.GetAvailableNozzlesAsync();
                return availableNozzles;
            }
            catch (AggregateException e)
            {
                _logger.Log(LogLevel.Error, "Something went wrong trying to get the nozzles from the spray module", e);
                HttpContext.Response.StatusCode = 500;
                return Json(new ErrorResponse("Something went wrong trying to get the nozzles from the spray module"));
            }
        }

        /// <summary>
        /// Update the nozzles in the system i.e. if nozzles should start or stop spraying, tell the spraying module
        /// via IPC.
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        public async Task<ActionResult<SuccessResponse>> UpdateNozzles([FromBody] List<Nozzle> patchedNozzles)
        {
            patchedNozzles.ForEach(nozzle => _logger.Log(LogLevel.Information, $"Patched nozzle: {nozzle.DisplayName} {nozzle.ShouldBeSpraying}"));
            try
            {
                await _sprayModuleBridge.UpdateNozzleStatusesAsync(patchedNozzles);
                return new SuccessResponse("Asked spray module to update the nozzles");
            }
            catch (AggregateException e)
            {
                _logger.Log(LogLevel.Error, "Something went wrong trying to update the nozzles via the spray module", e);
                HttpContext.Response.StatusCode = 500;
                return Json(new ErrorResponse("Something went wrong trying to update the nozzles via the spray module"));
            }
        }
    }
}