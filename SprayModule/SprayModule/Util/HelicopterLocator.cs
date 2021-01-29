using System;
using System.Collections.Generic;
using NumSharp;
using SprayModule.Exception;
using SprayModule.Model;

namespace SprayModule.Util
{
    /// <summary>
    /// Class to provide functionality to locate the helicopter inside the spray matrix
    /// </summary>
    public class HelicopterLocator
    {
        private readonly NZTMPoint[,] locationMatrix;
        private readonly NDArray sprayMatrix;
        
        
        public HelicopterLocator(NZTMPoint[,] locationMatrix, NDArray sprayMatrix)
        {
            this.locationMatrix = locationMatrix;
            this.sprayMatrix = sprayMatrix;
        }

        /// <summary>
        /// Method to find the index of the helicopter inside the spray location matrix
        /// </summary>
        /// <param name="helicopterPosition">The position of the helicopter in NZTM coordinates</param>
        /// <exception cref="HelicopterOutOfSprayAreaException">Throws exception when helicopter is out of spray area</exception>
        /// <returns>Tuple of the 2-dimensional index of the helicopter</returns>
        public (int, int) GetHelicopterIndex(NZTMPoint helicopterPosition)
        {
            var xSize = locationMatrix.GetLength(1);
            var ySize = locationMatrix.GetLength(0);
            
            var topLeft = locationMatrix[0, 0];
            var topRight = locationMatrix[0, xSize - 1];
            var botLeft = locationMatrix[ySize - 1, 0];
            var botRight = locationMatrix[ySize - 1, xSize - 1];
            

            if (!CheckHelicopterInsideSprayArea(topLeft, botRight, helicopterPosition))
            {
                throw new HelicopterOutOfSprayAreaException("Helicopter not within spray area");
            }
            
            var xStep = (int) Math.Ceiling(Math.Abs(topLeft.x - topRight.x) / xSize);
            var yStep = (int) Math.Ceiling(Math.Abs(topLeft.y - botLeft.y) / ySize);

            if (xStep == 0 || yStep == 0)
            {
                throw new InvalidMatrixStepException("Step value was found to be 0");
            }

            var xIndex = GetXIndex(helicopterPosition, xStep, topLeft.x);
            var yIndex = GetYIndex(helicopterPosition, yStep, topLeft.y);
            return (xIndex, yIndex);
        }

        /// <summary>
        /// Method to find the x index of the helicopter
        /// </summary>
        /// <param name="xStep">Change in x between each column</param>
        /// <param name="xOrigin">X coordinate of the top left item of the matrix</param>
        /// /// <param name="helicopterPosition">The position of the helicopter in NZTM coordinates</param>
        /// <returns>The x index of the helicopter</returns>
        private int GetXIndex(NZTMPoint helicopterPosition, int xStep, double xOrigin)
        {
            var diff = Math.Abs(helicopterPosition.x - xOrigin);
            return (int) Math.Floor(diff / xStep);
        }

        /// <summary>
        /// Method to find the y index of the helicopter
        /// </summary>
        /// <param name="yStep">Change in y between each row</param>
        /// <param name="yOrigin">Y coordinate of the top left item of the matrix</param>
        /// <param name="helicopterPosition">The position of the helicopter in NZTM coordinates</param>
        /// <returns>The y index of the helicopter</returns>
        private int GetYIndex(NZTMPoint helicopterPosition, int yStep, double yOrigin)
        {
            var diff = Math.Abs(helicopterPosition.y - yOrigin);
            return (int)Math.Floor(diff / yStep);
        }

        /// <summary>
        /// Method to check if the helicopter is inside the spray area matrix
        /// </summary>
        /// <param name="topLeft">The NZTM point in the top left of the matrix</param>
        /// <param name="botRight">The NZTM point in the bottom right of the matrix</param>
        /// <param name="helicopterPosition">The helicopter position in NZTM coordinates</param>
        /// <returns>boolean true if the helicopter is inside the spray area</returns>
        private bool CheckHelicopterInsideSprayArea(NZTMPoint topLeft, NZTMPoint botRight,
            NZTMPoint helicopterPosition)
        {
            return !(helicopterPosition.x < topLeft.x) && !(helicopterPosition.x > botRight.x) && 
                   !(helicopterPosition.y > topLeft.y) && !(helicopterPosition.y < botRight.y);
        }
        
        /// <summary>
        /// Method to get a local spray matrix centered around the helicopters current position
        /// </summary>
        /// <param name="index1">The first index of the helicopter in the 2 d spray matrix</param>
        /// <param name="index2">The first index of the helicopter in the 2 d spray matrix</param>
        /// <param name="width">The of the local matrix</param>
        /// <param name="height">The height of the local matrix</param>
        /// <returns>A tuple with the first element being the local matrix and the second element is a tuple holding the
        /// helicopter index in the local matrix</returns>
        public (NDArray, (int, int)) GetSubMatrix(int index1, int index2, int width, int height)
        {
            var tempLower1 = index1 - height;
            var tempLower2 = index2 - width;
            var shape = sprayMatrix.shape;
            var lowerIndex1 = Math.Max(0, tempLower1);
            var upperIndex1 = Math.Min(shape[0], index1 + height);

            var lowerIndex2 = Math.Max(0, tempLower2);
            var upperIndex2 = Math.Min(shape[1], index2 + width);

            return (sprayMatrix[$"{lowerIndex1}:{upperIndex1},{lowerIndex2}:{upperIndex2}"], 
                GetSubMatrixIndex(tempLower1, tempLower2, index1, index2, height, width));
        }

        /// <summary>
        /// Method to calculate the index of the helicopter within the localised spray matrix
        /// </summary>
        /// <param name="tempLower1">The lower bound of the y index of the matrix</param>
        /// <param name="tempLower2">The lower bound of the x index of the matrix</param>
        /// <param name="index1">The y index of the helicopter in the full matrix</param>
        /// <param name="index2">The x index of the helicopter in the full matrix</param>
        /// <param name="height">The height of the sub matrix</param>
        /// <param name="width">The width of the sub matrix</param>
        /// <returns>Tuple of the index in the sub matrix</returns>
        private (int, int) GetSubMatrixIndex(int tempLower1, int tempLower2, int index1, int index2, int height, int width)
        {
            var localIndex1 = 0;
            var localIndex2 = 0;
            
            if (tempLower1 == 0)
            {
                localIndex1 = index1;
            } else if (tempLower1 < 0)
            {
                localIndex1 = height + (index1 - height);
            }
            else
            {
                localIndex1 = height;
            }
            
            if (tempLower2 == 0)
            {
                localIndex2 = index2;
            } else if (tempLower2 < 0)
            {
                localIndex2 = width + (index2 - width);
            }
            else
            {
                localIndex2 = width;
            }

            return (localIndex1, localIndex2);
        }
    }
}