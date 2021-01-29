using System.Collections.Generic;
using WildingPines.Io.Exceptions;
using WildingPines.Models;

namespace WildingPines.Io.Reports
{
    public interface IUsbReportsExporter
    {
        /// <summary>
        /// Write the given reports to a specified USB.
        /// </summary>
        /// <param name="reports"></param>
        /// <param name="usbName"></param>
        /// <exception cref="UsbNotFoundException"></exception>
        public void WriteReportsToUsb(List<SprayReport> reports, string usbName);
    }
}