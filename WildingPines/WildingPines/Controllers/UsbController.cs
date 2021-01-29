using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WildingPines.Io;
using WildingPines.Io.Usb;
using WildingPines.Models;

namespace WildingPines.Controllers
{
    /// <summary>
    /// Manages USB operations that are task independent, such as checking available drives,
    /// getting available drives, etc.
    /// </summary>
    [Route("api/v1/usb")]
    public class UsbController : Controller
    {
        private readonly UsbIo _usbIo = new UsbIo();
        
        /// <summary>
        /// Return all the available USB devices, i.e. USB drives connected
        /// with the appropriate name convention.
        /// </summary>
        /// <returns></returns>
        [Route("available")]
        [HttpGet]
        public List<UsbDrive> GetAllAvailableUsbDrives(string usbName)
        {
            return _usbIo.GetAllAvailableUsbDrives();
        }
    }
}