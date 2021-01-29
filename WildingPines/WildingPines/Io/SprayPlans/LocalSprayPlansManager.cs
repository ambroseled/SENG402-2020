using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using WildingPines.Models;

namespace WildingPines.Io.SprayPlans
{
    public class LocalSprayPlansManager : ILocalSprayPlansManager
    {
        private readonly IUsbSprayPointsImporter _usbSprayPointsImporter;
        private readonly ILogger _logger;

        public LocalSprayPlansManager(IUsbSprayPointsImporter usbSprayPointsImporter, ILogger<LocalSprayPlansManager> logger)
        {
            _usbSprayPointsImporter = usbSprayPointsImporter;
            _logger = logger;
        }

        /// <inheritdoc cref="ILocalSprayPlansManager"/> />
        public List<SprayPlan> GetSprayPlansInNuc()
        {
            return Directory
                .EnumerateFiles(_usbSprayPointsImporter.SprayPointFilesDirectory)
                .Where(filename => _usbSprayPointsImporter.IsSprayPointsFile(filename))
                .Select(filename => new SprayPlan(filename))
                .ToList();
        }
    }
}