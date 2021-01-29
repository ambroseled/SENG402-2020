using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using System.Linq;
using WildingPines.Models;

namespace WildingPines.Util
{
    /// <summary>
    /// An implementation of the FileReader interface to read SprayPoints from a
    /// CSV (Comma Separated Values) file. This filereader is used for the
    /// implementation of an MVP only.
    /// </summary>
    [Obsolete]
    public class CsvFileReader
    {
        /// <summary>
        /// An implementation of the getSprayPoint() method which uses CsvHelper
        /// to read SprayPoint objects out of a csv file.
        /// </summary>
        /// <param name="fileName">A csv filename containing tree locations</param>
        /// <returns>A List object containing all the SprayPoint objects read
        /// from the passed file</returns>
        public List<SprayPoint> getSprayPoints(string fileName)
        {
            using var reader = new StreamReader(augmentCurrentDirectory(fileName));
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();
            IEnumerable<SprayPoint> records = csv.GetRecords<SprayPoint>();
            return records.ToList();
        }

        /// <summary>
        /// Combines a passed file name with the current working directory to
        /// allow getSprayPoints() to read the file.
        /// </summary>
        /// <param name="fileName">The string of the file name to combine</param>
        /// <returns>A string containing the full path to the file passed</returns>
        private string augmentCurrentDirectory(string fileName)
        {
            string path = Directory.GetCurrentDirectory();
            path += "/../../../TestData/";
            path += fileName;
            return path;
        }


        /// <summary>
        /// Reads a GPS path put of a given csv file and returns a list of the locations
        /// </summary>
        /// <param name="filename">The csv filename</param>
        /// <returns>List of GpsLocation objects</returns>
        public List<GpsLocation> getGpsPath(string fileName)
        {
            using var reader = new StreamReader(augmentCurrentDirectory(fileName));
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();
            IEnumerable<GpsLocation> records = csv.GetRecords<GpsLocation>();
            return records.ToList();
        }
    }
}
