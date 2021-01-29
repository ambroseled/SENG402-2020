using System;
using System.Collections.Generic;
using WildingPines.Models;

namespace WildingPines.Io.Usb
{
    public interface IUsbIo
    {
        /// <summary>
        /// Determines whether the USB with that name is available.
        /// </summary>
        /// <param name="usbName"></param>
        /// <returns></returns>
        public bool IsUsbAvailable(string usbName);
        
        /// <summary>
        /// Get all available USB drives with the expected naming convention.
        /// </summary>
        /// <returns>All available USB drives with the expected naming convention.</returns>
        public List<UsbDrive> GetAllAvailableUsbDrives();
        
        /// <summary>
        /// Returns the file names for all files which name matches the predicate.
        /// </summary>
        /// <param name="usbName"></param>
        /// <param name="nameFilter">A function that returns true if a file with that name is wanted.</param>
        /// <returns></returns>
        public List<string> GetFilesInUsbDrive(string usbName, Predicate<string> nameFilter);
    }
}