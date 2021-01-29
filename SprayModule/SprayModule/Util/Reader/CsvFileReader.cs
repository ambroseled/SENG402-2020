using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using SprayModule.Model;

namespace SprayModule.Util.Reader
{
    /// <summary>
    /// An implementation of the FileReader interface to read SprayPoints from a
    /// CSV (Comma Separated Values) file. This filereader is used for the
    /// implementation of an MVP only.
    /// </summary>
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
            using var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);
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
            using var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();
            IEnumerable<GpsLocation> records = csv.GetRecords<GpsLocation>();
            return records.ToList();
        }


        /// <summary>
        /// Method to read in data used for project demo
        /// </summary>
        /// <param name="fileName">The path to the test data</param>
        /// <returns>2d array of GpsLocation objects and 2d array of 0/1 as spray values</returns>
        [Obsolete]
        public (GpsLocation[,], int[,]) GetDemoSprayPoints(string fileName)
        {
            var points = File.ReadLines(fileName).Select(line => 
                (from point in line.Split(",") select point.Split(":")
                    into data let gpsLocation = new GpsLocation(Convert.ToDouble(data[0]), 
                        Convert.ToDouble(data[1])) select (gpsLocation, Convert.ToInt32(data[2]))
                    ).ToList()).ToList();
            var height = points.Count;
            var width = points[0].Count;
            var locationMatrix = new GpsLocation[height, width];
            var sprayMatrix = new int[height, width];

            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var point = points[i][j];
                    locationMatrix[i, j] = point.gpsLocation;
                    sprayMatrix[i, j] = point.Item2;
                    if (point.Item2 == 1)
                    {
                        Console.WriteLine($"{point.gpsLocation.Latitude}, {point.gpsLocation.Longitude}");
                    }
                }
            }

            return (locationMatrix, sprayMatrix);
        }


        /// <summary> 
        /// Method to read in the demo data and produce a list of tree locations
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>List of double tuple holding lat/lng of trees</returns>
        public List<(double, double)> GetDemoTreeLocations(string fileName)
        {
            using var reader = new StreamReader(fileName);
            using var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();
            return csv.GetRecords<GpsLocation>().Select(location => (location.Latitude, location.Longitude)).ToList();
        }
    }
}
