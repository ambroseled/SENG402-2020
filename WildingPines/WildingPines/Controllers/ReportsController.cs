using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WildingPines.Controllers.Responses;
using WildingPines.Io;
using WildingPines.Io.Exceptions;
using WildingPines.Io.Reports;
using WildingPines.Models;
using WildingPines.Models.Responses;

namespace WildingPines.Controllers
{
    /// <summary>
    /// Manages end points to do with creating and exporting reports after flying.
    /// </summary>
    /// TODO: WP-67: export actual generated reports
    [Route("api/v1/reports")]
    public class ReportsController : Controller
    {
        private readonly ILogger _logger;
        private readonly IUsbReportsExporter _usbReportsExporter;
        private readonly List<SprayReport> _reports = new List<SprayReport>();

        public ReportsController(ILogger<ReportsController> logger, IUsbReportsExporter usbReportsExporter)
        {
            _logger = logger;
            _usbReportsExporter = usbReportsExporter;
            _reports.Add(new SprayReport("Ilam Fields", 12, 24, 48));
            _reports.Add(new SprayReport("Taranaki", 12, 24, 48));
        }
        
        /// <summary>
        /// Returns which reports are available in the NUC.
        /// Could be used to populate the user interface with a list of reports that could be exported to a USB.
        /// </summary>
        /// <returns>the reports available in the NUC</returns>
        [HttpGet]
        public List<SprayReport> GetAvailableReportsInNuc()
        {
            return _reports;
        }

        /// <summary>
        /// Export specific reports to a USB.
        /// </summary>
        /// <param name="usbName"></param>
        /// <param name="reportNames">the names of the reports that we want to export.</param>
        /// <returns></returns>
        [Route("export/usb")]
        [HttpPost]
        public ActionResult<SuccessResponse> ExportReportsToUsb(string usbName, [FromBody] List<string> reportNames)
        {
            var reportsToExport = _reports.FindAll(report => reportNames.Contains(report.Name));
            _logger.Log(LogLevel.Information, $"Exporting reports {string.Join(", ", reportNames)} to USB {usbName}");
            try
            {
                _usbReportsExporter.WriteReportsToUsb(reportsToExport, usbName);
                _logger.Log(LogLevel.Information, $"Wrote reports to USB with name {usbName}");
                return new SuccessResponse($"Wrote reports to USB with name {usbName}");
            }
            catch (UsbNotFoundException usbNotFoundException)
            {
                _logger.Log(LogLevel.Warning, usbNotFoundException.Message);
                HttpContext.Response.StatusCode = 404;
                return Json(new ErrorResponse($"USB with name {usbName} was not available"));
            }
        }

        /// <summary>
        /// Exports all reports to the USB with the name provided, given
        /// that the USB is available.
        /// Maps to <code>reports/export/all/usb?usbname=something</code>
        /// </summary>
        /// <param name="usbName"></param>
        [Route("export/all/usb")]
        [HttpPost]
        public ActionResult<SuccessResponse> ExportAllReportsToUsb(string usbName)
        {
            try
            {
                _usbReportsExporter.WriteReportsToUsb(_reports, usbName);
                _logger.Log(LogLevel.Information, $"Wrote reports to USB with name {usbName}");
                return new SuccessResponse($"Wrote reports to USB with name {usbName}");
            }
            catch (UsbNotFoundException usbNotFoundException)
            {
                _logger.Log(LogLevel.Warning, usbNotFoundException.Message);
                HttpContext.Response.StatusCode = 404;
                return Json(new ErrorResponse($"USB with name {usbName} was not available"));
            }
        }
    }
}