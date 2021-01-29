using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using Microsoft.Extensions.Logging;
using WildingPines.Io.Exceptions;
using WildingPines.Io.Usb;
using WildingPines.Models;

namespace WildingPines.Io.SprayPlans
{
    /// <summary>
    /// Imports spray points locations from a USB.
    /// </summary>
    public class UsbSprayPointsImporter : IUsbSprayPointsImporter
    {
        private readonly Predicate<string> _isSprayPointsFile = filename => filename.Contains("csv");
        private readonly ILogger _logger;
        private readonly IUsbIo _usbIo;

        private readonly string _sprayPointFilesDirectory =
            $"{Directory.GetCurrentDirectory()}/{UsbIo.LocalFilesDirectory}/spray_points";

        public UsbSprayPointsImporter(ILogger<UsbSprayPointsImporter> logger, IUsbIo usbIo)
        {
            _logger = logger;
            _usbIo = usbIo;
            Directory.CreateDirectory(_sprayPointFilesDirectory);
        }

        /// <inheritdoc cref="IUsbSprayPointsImporter"/>
        public void ImportSprayPointsFile(string usbName, string filePath)
        {
            if (!_usbIo.IsUsbAvailable(usbName))
            {
                throw new UsbNotFoundException($"No USB contained {UsbIo.ExpectedStringInUsbName} in its name");
            }

            var matchingFilePaths = _usbIo.GetFilesInUsbDrive(usbName, usbFileName => usbFileName.Contains(filePath));
            if (matchingFilePaths.Count.Equals(0))
            {
                throw new SprayPointsFileNotFoundException(filePath);
            }

            var fileName = Path.GetFileName(filePath);
            var destinationPath = $"{_sprayPointFilesDirectory}/{fileName}";
            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }
            File.Copy(matchingFilePaths.First(), destinationPath);
        }

        /// <inheritdoc cref="IUsbSprayPointsImporter" />
        public List<SprayPlan> GetSprayPlansForUsb(string usbName)
        {
            if (!_usbIo.IsUsbAvailable(usbName))
            {
                throw new UsbNotFoundException($"No USB contained {UsbIo.ExpectedStringInUsbName} in its name");
            }

            return _usbIo.GetFilesInUsbDrive(usbName, _isSprayPointsFile)
                .Select(filename => new SprayPlan(filename))
                .ToList();
        }

        /// <inheritdoc cref="IUsbSprayPointsImporter" />
        public string SprayPointFilesDirectory => _sprayPointFilesDirectory;

        /// <inheritdoc cref="IUsbSprayPointsImporter"/>/>
        public Predicate<string> IsSprayPointsFile => _isSprayPointsFile;

            /// <inheritdoc cref="IUsbSprayPointsImporter"/>
        public void ImportAllSprayPointFiles(string usbName)
        {
            if (!_usbIo.IsUsbAvailable(usbName))
            {
                throw new UsbNotFoundException($"No USB contained {UsbIo.ExpectedStringInUsbName} in its name");
            }
            
            var filePaths = _usbIo.GetFilesInUsbDrive(usbName, _isSprayPointsFile);
            filePaths.ForEach(filePath =>
            {
                var fileName = Path.GetFileName(filePath);
                var destinationPath = $"{_sprayPointFilesDirectory}/{fileName}";
                if (File.Exists(destinationPath))
                {
                    File.Delete(destinationPath);
                }
                File.Copy(filePath, destinationPath);
            });
        }

        // TODO: use when implementing endpoint that needs to read spray points.
        /// <inheritdoc cref="IUsbSprayPointsImporter"/>
        public List<SprayPoint> ReadSprayPointsFromFile(string fileName)
        {
            var pathToFile = $"{_sprayPointFilesDirectory}/{fileName}";
            using var reader = new StreamReader(pathToFile);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            try
            {
                var records = csv.GetRecords<SprayPoint>();
                return records.ToList();
            }
            catch (CsvHelperException csvHelperException)
            {
                _logger.Log(LogLevel.Warning, $"Could not read spray points from file in the NUC. Reason: {csvHelperException.Message}");
                throw new SprayPointsFileReadException($"Could not read the spray points in the file with the path {pathToFile} in the NUC");
            }
        }
    }
}