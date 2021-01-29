using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WildingPines.Communication.SprayModuleIpc;
using WildingPines.Controllers.Responses;
using WildingPines.Models.Responses;
using WildingPines.Models;

namespace WildingPines.Controllers
{
    /// <summary>
    /// Controller for the api/v1/fly endpoint
    /// </summary>
    [Route("api/v1")]
    public class FlightController : Controller
    {
        private readonly ILogger _logger;
        private readonly ISprayModuleBridge _sprayModuleBridge;

        public FlightController(ILogger<FlightController> iLogger, ISprayModuleBridge sprayModuleBridge)
        {
            _logger = iLogger;
            _sprayModuleBridge = sprayModuleBridge;
        }
        
        /// <summary>
        /// Endpoint to allow the main spray loop to be invoked on a thread pool thread
        /// </summary>
        /// <returns>SuccessFailResponse object</returns>
        [Route("fly")]
        [HttpPost]
        public async Task<ActionResult<SuccessResponse>> PostEnterFlightState([FromBody] SprayPlan sprayPlan)
        {
            _logger.Log(LogLevel.Information, $"Received file path from client:{sprayPlan.Name}");
            var changedStateSuccessfully = await _sprayModuleBridge.SetSprayModuleToFlyingState(sprayPlan.Name);
            if (changedStateSuccessfully)
            {
                _logger.Log(LogLevel.Information, $"Spray module state changed to flying");
                return new SuccessResponse($"The system is now ready to spray");   
            }
            HttpContext.Response.StatusCode = 500;
            return Json(new ErrorResponse("Something went wrong setting the spray module to flying mode"));
        }

        /// <summary>
        /// Endpoint to stop the spray loop
        /// </summary>
        /// <returns>SuccessFailResponse object</returns>
        [Route("land")]
        [HttpPost]
        public async Task<ActionResult<SuccessResponse>> PostEnterLandState()
        {
            var changedStateSuccessfully = await _sprayModuleBridge.SetSprayModuleToLandedState();
            if (changedStateSuccessfully)
            {
                _logger.Log(LogLevel.Information, $"Spray module state changed to landed");
                return new SuccessResponse($"The system is now set to landed");   
            }
            HttpContext.Response.StatusCode = 500;
            return Json(new ErrorResponse("Something went wrong setting the spray module to landed mode"));
        }

        /// <summary>
        /// Method to generate a path through the spray matrix
        /// </summary>
        /// <param name="startX">X origin</param>
        /// <param name="startY">Y origin</param>
        /// <param name="width">Width of the matrix</param>
        /// <param name="height">Height of the matrix</param>
        /// <returns></returns>
        private IEnumerable<NZTMPoint> GetHelicopterPath(double startX, double startY, int width, int height)
        {
            var path = new List<NZTMPoint>();
            startX += 402;
            for (var i = 0; i < height; i++)
            {
                path.Add(new NZTMPoint(startX, startY, 0));
                startY--;
            }

            return path;
        }
    }
}