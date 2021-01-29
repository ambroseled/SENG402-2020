using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WildingPines.Controllers.Responses;
using WildingPines.Io;
using WildingPines.Io.Exceptions;
using WildingPines.Io.SprayPlans;
using WildingPines.Models;
using WildingPines.Models.Responses;

namespace WildingPines.Controllers
{
    /// <summary>
    /// Manages endpoints to do with spray points. For example, importing these from files in a USB.
    /// </summary>
    [Route("api/v1/plans")]
    public class SprayPointsController : Controller
    {
        private readonly ILogger _logger;
        private readonly IUsbSprayPointsImporter _usbSprayPointsImporter;
        private readonly ILocalSprayPlansManager _localSprayPlansManager;

        public SprayPointsController(ILogger<SprayPointsController> logger, IUsbSprayPointsImporter usbSprayPointsImporter, ILocalSprayPlansManager localSprayPlansManager)
        {
            _logger = logger;
            _usbSprayPointsImporter = usbSprayPointsImporter;
            _localSprayPlansManager = localSprayPlansManager;
        }

        /// <summary>
        /// Get the available spray plans in the NUC.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<List<SprayPlan>> GetAvailableSprayPlansInNuc()
        {
            return _localSprayPlansManager.GetSprayPlansInNuc();
        }
        
        /// <summary>
        /// Get the available spray plans in a specific USB.
        /// </summary>
        /// <param name="usbName"></param>
        /// <returns></returns>
        [Route("usb")]
        [HttpGet]
        public ActionResult<List<SprayPlan>> GetAvailableSprayPlansInUsb(string usbName)
        {
            try
            {
                var availableSprayPlans = _usbSprayPointsImporter.GetSprayPlansForUsb(usbName);
                _logger.Log(LogLevel.Information,
                    $"Returned {availableSprayPlans.Count} available plans for USB with name: {usbName}");
                return availableSprayPlans;
            }
            catch (UsbNotFoundException usbNotFoundException)
            {
                _logger.Log(LogLevel.Warning, usbNotFoundException.Message);
                return NotFound($"Could not find USB with name {usbName}");
            }
        }

        /// <summary>
        /// Imports all flight plans from the USB with the name provided, given
        /// that the USB is available.
        /// Maps to <code>plans/import/all/usb?usbname=something</code>
        /// </summary>
        /// <param name="usbName"></param>
        [Route("import/all/usb")]
        [HttpPost]
        public ActionResult<SuccessResponse> ImportAllSprayPointFilesFromUsb(string usbName)
        {
            try
            {
                _usbSprayPointsImporter.ImportAllSprayPointFiles(usbName);
                _logger.Log(LogLevel.Information, $"Imported all spray point files from USB with name {usbName}");
                return new SuccessResponse($"Imported all spray point files from USB with name {usbName}");
            }
            catch (UsbNotFoundException usbNotFoundException)
            {
                _logger.Log(LogLevel.Warning, usbNotFoundException.Message);
                HttpContext.Response.StatusCode = 404;
                return Json(new ErrorResponse($"USB with name {usbName} was not available"));
            }
        }

        /// <summary>
        /// Import spray plans from a USB to the NUC.
        /// </summary>
        /// <param name="usbName">the name of the USB to import the spray plans from.</param>
        /// <param name="filePaths">the paths of the files to import.</param>
        /// <returns></returns>
        [Route("import/usb")]
        [HttpPost]
        public ActionResult<SuccessResponse> ImportSprayPointFilesFromUsb(string usbName, [FromBody] List<string> filePaths)
        {
            try
            {
                filePaths.ForEach(filePath => _usbSprayPointsImporter.ImportSprayPointsFile(usbName, filePath));
                _logger.Log(LogLevel.Information, $"Imported spray point files {string.Join(", ", filePaths)} from USB with name {usbName}");
                return new SuccessResponse($"Imported spray points files {string.Join(", ", filePaths)} from USB with name {usbName}");
            }
            catch (UsbNotFoundException usbNotFoundException)
            {
                _logger.Log(LogLevel.Warning, usbNotFoundException.Message);
                HttpContext.Response.StatusCode = 404;
                return Json(new ErrorResponse($"USB with name {usbName} was not available"));
            }
            catch (SprayPointsFileNotFoundException sprayPointsFileNotFound)
            {
                _logger.Log(LogLevel.Warning, sprayPointsFileNotFound.Message);
                HttpContext.Response.StatusCode = 404;
                return Json(new ErrorResponse($"File with name {sprayPointsFileNotFound.FileName} was not available in USB with name {usbName}"));
            }
        }
    }
}