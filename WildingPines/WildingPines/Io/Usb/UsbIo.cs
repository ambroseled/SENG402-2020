using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WildingPines.Models;

namespace WildingPines.Io.Usb
{

    /// <summary>
    /// A class to do IO with a USB.
    /// </summary>
    public class UsbIo : IUsbIo
    {
        public const string ExpectedStringInUsbName = "DOC-WP";
        public const string LocalFilesDirectory = "local_files";

        /// <inheritdoc cref="IUsbIo"/>
        public bool IsUsbAvailable(string usbName)
        {
            return DriveInfo.GetDrives()
                .Any((driveInfo) => driveInfo.Name.Equals(usbName));
        }

        /// <inheritdoc cref="IUsbIo"/>
        public List<UsbDrive> GetAllAvailableUsbDrives()
        {
            return DriveInfo.GetDrives()
                .Where(DriveNameMatchesConvention)
                .Select((driveInfo) => new UsbDrive(driveInfo.VolumeLabel)).ToList();
        }
        
        /// <inheritdoc cref="IUsbIo"/>
        public List<string> GetFilesInUsbDrive(string usbName, Predicate<string> nameFilter)
        {
            return Directory.EnumerateFiles(usbName).Where(filename => nameFilter(filename)).ToList();
        }

        /// <summary>
        /// Returns whether the drive name matches the expected naming
        /// convention of containing "DOC-WP" in the USB name.
        /// </summary>
        /// <param name="driveInfo"></param>
        /// <returns></returns>
        private bool DriveNameMatchesConvention(DriveInfo driveInfo) =>
            driveInfo.VolumeLabel.ToLower()
                .Contains(ExpectedStringInUsbName.ToLower());
    }
}
