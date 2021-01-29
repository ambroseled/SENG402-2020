using System;
using System.Collections.Generic;
using System.IO;
using NumSharp;
using OSGeo.GDAL;
using SprayModule.Model;

namespace SprayModule.Util.Reader
{
    /// <summary>
    /// Implementation of the IFileReader interface to read in spray data from a raster (.tif) file
    /// </summary>
    class RasterFileReader : IFileReader
    {
        private int unexpectedSprayValue = -9999;
        
        /// <summary>
        /// Method to read spray point data out of a given raster (.tif) image. Pixels are encoded with a x, y values and a spray ratio.
        /// Data points are returned in the NZTM2000 coordinate system
        /// </summary>
        /// <param name="fileName">The filename</param>
        /// <returns>The converted list of NZTMPoint objects</returns>
        public (NZTMPoint[,], NDArray) GetNztmSprayPoints(string fileName)
        {
            var (band, meta) = SetUpGdal(fileName);
            return ConvertBandNztm((int) meta[0], (int) meta[1], meta[2], meta[3], meta[4],
                band);
        }
        
        /// <summary>
        /// Method to read spray point data out of a given raster (.tif) image. Pixels are encoded with a x, y values and a spray ratio.
        /// Data points are returned in the WGS84 coordinate system
        /// </summary>
        /// <param name="fileName">The filename</param>
        /// <returns>The converted list of SprayPoint object</returns>
        [Obsolete]
        public List<List<SprayPoint>> GetWgs84SprayPoints(string fileName)
        {
            var (band, meta) = SetUpGdal(fileName);
            return ConvertBandWgs84((int) meta[0], (int) meta[1], meta[2], meta[3], meta[4],
                band);
        }

        /// <summary>
        /// Method to read spray point data into a boolean spray matrix
        /// </summary>
        /// <param name="fileName">The filename of the raster image</param>
        /// <returns>Matrix in form of list of lists of ints (0 or 1)</returns>
        [Obsolete]
        public NDArray GetBooleanSprayMatrix(string fileName)
        {
            var (band, meta) = SetUpGdal(fileName);
            var points = new int[(int) meta[0], (int) meta[1]];
            for (var k = 0; k < meta[0]; k++) 
            {
                var y = meta[3] - k * meta[4]; 
                var buf = new byte[(int)meta[1]];
                band.ReadRaster(0, k, (int)meta[1], 1, buf, (int)meta[1], 1, 0, 0);
                //var row = new List<int>();
                for (var r = 0; r < meta[1]; r++)
                {
                    if (buf[r] == unexpectedSprayValue) continue;
                    //row.Add(buf[r]);
                    points[k, r] = buf[r];
                }
                //points.Add(row);
            }
            NDArray output = points;
            return output;
        }

        /// <summary>
        /// Helper function to setup GDAL and read the meta and band data out of the raster image
        /// </summary>
        /// <param name="fileName">Raster filename</param>
        /// <returns>Tuple of a Band object and a list of doubles as (rows, cols, startX, startY, interval)</returns>
        private (Band, List<double>) SetUpGdal(string fileName)
        {
            Gdal.AllRegister(); //Register all Gdal data drives
            var ds = Gdal.Open(fileName, Access.GA_ReadOnly);
            var gt = new double[6];
            ds.GetGeoTransform(gt); //Read geo transform info into array
            var metaData = new List<double>
            {
                ds.RasterYSize, // rows
                ds.RasterXSize, // cols
                gt[0], // startX
                gt[3], // startY
                gt[1] // interval
            };
            return (ds.GetRasterBand(1), metaData);
        }

        /// <summary>
        /// Method to process a band from a raster image into a list of NZTM objects.
        /// </summary>
        /// <param name="rows">The number of rows in the band</param>
        /// <param name="cols">The number of columns in the band</param>
        /// <param name="startX">The starting x coordinate</param>
        /// <param name="startY">The starting y coordinate</param>
        /// <param name="interval">The cell size of cells in the band</param>
        /// <param name="band">The band to process</param>
        /// <returns>A list holding NZTM points from the band</returns>
        private (NZTMPoint[,], NDArray) ConvertBandNztm(int rows, int cols, double startX, double startY, double interval, 
            Band band)
        {
            var points = new NZTMPoint[rows, cols];
            var boolPoints = new int[rows, cols];
            for (var k = 0; k < rows; k++) 
            {
                var y = startY - k * interval; 
                var buf = new byte[cols];
                band.ReadRaster(0, k, cols, 1, buf, cols, 1, 0, 0);
                for (var r = 0; r < cols; r++)
                {
                    if (buf[r] == unexpectedSprayValue) continue;
                    var x = startX + r * interval;
                    var point = new NZTMPoint(x, y, buf[r]);
                    points[k, r] = point;
                    boolPoints[k, r] = buf[r];
                }
            }
            return (points, (NDArray) boolPoints);
        }
        
        /// <summary>
        /// Method to process a band from a raster image into a list of SprayPoint objects.
        /// </summary>
        /// <param name="rows">The number of rows in the band</param>
        /// <param name="cols">The number of columns in the band</param>
        /// <param name="startX">The starting x coordinate</param>
        /// <param name="startY">The starting y coordinate</param>
        /// <param name="interval">The cell size of cells in the band</param>
        /// <param name="band">The band to process</param>
        /// <returns>A list holding spray points from the band</returns>
        [Obsolete]
        private List<List<SprayPoint>> ConvertBandWgs84(int rows, int cols, double startX, double startY, double interval, 
            Band band)
        {
            var converter = new CoordinateConverter();
            var points = new List<List<SprayPoint>>();
            for (var k = 0; k < rows; k++) 
            {
                var y = startY - k * interval; 
                var buf = new byte[cols];
                band.ReadRaster(0, k, cols, 1, buf, cols, 1, 0, 0);
                var row = new List<SprayPoint>();
                for (var r = 0; r < cols; r++)
                {
                    if (buf[r] == unexpectedSprayValue) continue;
                    var x = startX + r * interval;
                    var (lat, lon) = converter.NZTMtoLatLng(x, y);
                    var point = new SprayPoint(lat, lon, buf[r]);
                    row.Add(point);
                }
                points.Add(row);
            }
            return points;
        }

        /// <summary>
        /// Method to save a band of the raster image to a csv file. Used primarily for testing / development
        /// </summary>
        /// <param name="rows">The number of rows in the band</param>
        /// <param name="cols">The number of columns in the band</param>
        /// <param name="startX">The starting x coordinate</param>
        /// <param name="startY">The starting y coordinate</param>
        /// <param name="interval">The cell size of cells in the band</param>
        /// <param name="band">The band to process</param>
        [Obsolete]
        private void SaveToCsv(int rows, int cols, double startX, double startY, double interval, Band band)
        {
            var filePath = AugmentCurrentDirectory("out.csv");
            using var csv = new StreamWriter(filePath);
            for (var k = 0; k < rows; k++)  //read one line at a time
            {
                var y = startY - k * interval;    //Current lon and lat
                var buf = new int[cols];
                band.ReadRaster(0, k, cols, 1, buf, cols, 1, 0, 0);
                //iterate each item in one line
                for (var r = 0; r < cols; r++)
                {
                    if (buf[r] == unexpectedSprayValue) continue;
                    var x = startX + r * interval;    //Current lon and lat
                    var newLine = $"{x},{y},{buf[r]}{Environment.NewLine}";
                    csv.Write(newLine);
                }
            }
        }

        /// <summary>
        /// Combines a passed file name with the current working directory to
        /// allow getSprayPoints() to read the file.
        /// </summary>
        /// <param name="fileName">The string of the file name to combine</param>
        /// <returns>A string containing the full path to the file passed</returns>
        private string AugmentCurrentDirectory(string fileName)
        {
            return Directory.GetCurrentDirectory() + "/../../../TestData/" + fileName;
        }
    }
}
