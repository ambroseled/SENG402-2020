using System.Collections.Generic;
using SprayModule.Model;

namespace SprayModuleTests.Util
{
    /// <summary>
    /// Helper class for unit tests
    /// </summary>
    public static class TestHelper
    {
        /// <summary>
        /// Helper function to create a mock spray matrix to enable testing
        /// </summary>
        /// <param name="xOrigin"></param>
        /// <param name="yOrigin"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static NZTMPoint[,] MatrixCreator(int xOrigin, int yOrigin, int width, int height, int step)
        {
            var matrix = new NZTMPoint[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int k = 0; k < width; k++)
                {
                    matrix[i, k] = new NZTMPoint(xOrigin, yOrigin, 0);
                    xOrigin += step;
                }
                yOrigin -= step;
            }
            return matrix;
        }
    }
}