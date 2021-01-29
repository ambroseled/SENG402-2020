using System;
using System.Collections.Generic;
using WildingPines.Io.Exceptions;
using WildingPines.Models;

namespace WildingPines.Io.SprayPlans
{
    public interface IUsbSprayPointsImporter
    {
        /// <summary>
        /// Reads a spray points file that's already in the NUC.
        /// </summary>
        /// <param name="fileName"></param>
        /// <exception cref="SprayPointsFileReadException">When something went wrong loading the spray points in a file</exception>
        /// <returns>The spray points in the file</returns>
        public List<SprayPoint> ReadSprayPointsFromFile(string fileName);

        /// <summary>
        /// Copy all spray point files to the NUC disk.
        /// NOTE: Will overwrite files with the same name that are already in the NUC.
        /// Precondition: The USB with that name is available.
        /// </summary>
        /// <param name="usbName"></param>
        public void ImportAllSprayPointFiles(string usbName);

        /// <summary>
        /// Copy spray point files to the NUC disk.
        /// Precondition: The USB with that name is available.
        /// </summary>
        /// <param name="usbName">The name of the USB where the file is</param>
        /// <param name="filePath">The filepath for the file with the tree locations</param>
        /// <exception cref="UsbNotFoundException"></exception>
        /// <exception cref="SprayPointsFileNotFoundException"></exception>
        /// <returns></returns>
        public void ImportSprayPointsFile(string usbName, string filePath);

        /// <summary>
        /// Get the available spray plans in the USB.
        /// </summary>
        /// <param name="usbName">the name of the USB</param>
        /// <exception cref="UsbNotFoundException"></exception>
        /// <returns>a list of spray plans in the USB</returns>
        public List<SprayPlan> GetSprayPlansForUsb(string usbName);

        /// <summary>
        /// The directory where the spray plans are stored.
        /// </summary>
        public string SprayPointFilesDirectory { get; }
        
        /// <summary>
        /// Decides whether a filename is a spray points file or not.
        /// </summary>
        public Predicate<string> IsSprayPointsFile { get; }
    }
}