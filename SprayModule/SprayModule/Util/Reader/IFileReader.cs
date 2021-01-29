using System;
using System.Collections.Generic;
using NumSharp;
using SprayModule.Model;

namespace SprayModule.Util.Reader
{
    /// <summary>
    /// This interface defines the interactions that a filereader should implement.
    /// </summary>
    public interface IFileReader
    {
        /// <summary>
        /// Method to read the spray points out of the input file and into NZTM2000 coordinates
        /// </summary>
        /// <param name="fileName">The name of the file to be read, the file
        /// extension must be included</param>
        /// <returns>A List object containing all the SprayPoint objects read
        /// from the passed file</returns>
        public (NZTMPoint[,], NDArray) GetNztmSprayPoints(string fileName);


        /// <summary>
        /// Method to read the spray points out of the input file and into WGS84 coordinates
        /// </summary>
        /// <param name="fileName">The name of the file to be read, the file
        /// extension must be included</param>
        /// <returns>A List object containing all the SprayPoint objects read
        /// from the passed file</returns>
        [Obsolete]
        public List<List<SprayPoint>> GetWgs84SprayPoints(string fileName);


        /// <summary>
        /// Method to read out the boolean spray matrix related to the spray points in the input file
        /// </summary>
        /// <param name="fileName">The name of the file to be read, the file
        /// extension must be included</param>
        /// <returns>List of lists of int 0 or 1</returns>
        [Obsolete]
        public NDArray GetBooleanSprayMatrix(string fileName);

    }
}
