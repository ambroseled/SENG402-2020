using System.Collections.Generic;
using System.IO;
using WildingPines.Io.Exceptions;
using WildingPines.Io.Usb;
using WildingPines.Models;
using WildingPines.Util;

namespace WildingPines.Io.Reports
{
    /// <summary>
    /// Manages exporting reports to a USB.
    /// </summary>
    public class UsbReportsExporter : IUsbReportsExporter
    {
        private readonly IUsbIo _usbIo;
        
        public UsbReportsExporter(IUsbIo usbIo)
        {
            _usbIo = usbIo;
        }

        /// <inheritdoc cref="IUsbReportsExporter"/>
        public void WriteReportsToUsb(List<SprayReport> reports, string usbName)
        {
            if (_usbIo.IsUsbAvailable(usbName))
            {
                reports.ForEach((report) => WriteReportToUsb(report, usbName));
            }
            else
            {
                throw new UsbNotFoundException($"USB with name {usbName} was not found");
            }
        }

        /// <summary>
        /// Writes one report to the USB drive.
        /// Preconditions: the USB is available.
        /// </summary>
        private void WriteReportToUsb(SprayReport sprayReport, string usbName)
        {
            var serialisedReport = Serializer.ToJson(sprayReport);
            var path = $"{usbName}/{sprayReport.Name}_report.json";
            
            using var writer = new StreamWriter(path);
            writer.Write(serialisedReport);
        }
    }
}