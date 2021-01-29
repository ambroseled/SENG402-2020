using System;
using System.Collections.Generic;
using SprayModule.Model;

namespace SprayModule.External.Meteorological
{
    /// <summary>
    /// An implementation of the IMeteorologicalReader interface used to mock the data that comes from the
    /// meteorological unit
    /// </summary>
    [Obsolete]
    public class MockMeteorologicalReader : IMeteorologicalReader

    {
        private List<NZTMPoint> _helicopterPath;
        private int _currentPosition;
        

        /// <summary>
        /// Method to initialise the state of the Mock Meteorological Reader 
        /// </summary>
        /// <param name="startX">The X origin of the helicopter path</param>
        /// <param name="startY">The Y origin of the helicopter path</param>
        /// <param name="height">The height of the spray matrix</param>
        public void InitialiseHelicopterPath(double startX, double startY, int height)
        {
            _helicopterPath = GetHelicopterPath(startX, startY, height);
            _currentPosition = 0;
        }
        
        /// <summary>
        /// Method used to get the next current location of the helicopter
        /// </summary>
        /// <returns>NZTMPoint object of helicopter location</returns>
        public NZTMPoint GetCurrentLocation()
        {
            return _helicopterPath[_currentPosition++];
        }

        /// <summary>
        /// Method to get the angle of the helicopter through the spray matrix
        /// Will always return 90 to match the mocked helicopter path
        /// </summary>
        /// <returns>Double of angle</returns>
        public double GetCurrentAngle()
        {
            return 90; // TODO check with Mech that this is right, the path is top to bottom of matrix
        }

        /// <summary>
        /// Method to get the current wind speed in m/s
        /// Will always return 5 for testing/development purposes
        /// </summary>
        /// <returns>Double of wind speed in m/s</returns>
        public double GetCurrentWindSpeed()
        {
            return 5; // TODO check with MEch, assuming this is meters per second
        }
        
        /// <summary>
        /// Method to generate a path through the spray matrix
        /// </summary>
        /// <param name="startX">X origin</param>
        /// <param name="startY">Y origin</param>
        /// <param name="height">Height of the matrix</param>
        /// <returns>A list of NZTMPoint objects simulating a helicopter path through the current spray matrix</returns>
        private List<NZTMPoint> GetHelicopterPath(double startX, double startY, int height)
        {
            var path = new List<NZTMPoint>();
            startX += 402;
            for (var i = 0; i < height; i++)
            {
                path.Add(new NZTMPoint(startX, startY, 0));
                startY--;
            }

            return path;
        }

    }
}