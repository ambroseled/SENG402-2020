using System;
using SprayModule.Model;

namespace SprayModule.External.Meteorological
{
    /// <summary>
    /// Interface for the interaction with the meteorological unit
    /// </summary>
    [Obsolete]
    public interface IMeteorologicalReader
    {
        public NZTMPoint GetCurrentLocation();

        public double GetCurrentAngle();

        public double GetCurrentWindSpeed();
    }
}