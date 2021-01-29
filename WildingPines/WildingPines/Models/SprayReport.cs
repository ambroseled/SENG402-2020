using System;

namespace WildingPines.Models
{
    /// <summary>
    /// A report that may be exported after flying.
    /// </summary>
    public class SprayReport
    {
        public SprayReport(string name, long sprayUsed, long flightTime, long treesSprayed)
        {
            Name = name;
            SprayUsed = sprayUsed;
            FlightTime = flightTime;
            TreesSprayed = treesSprayed;
            Created = DateTime.Now;
        }

        public string Name { get; }
        public DateTime Created { get; }
        
        public long SprayUsed { get; }
        
        public long FlightTime { get; }
        
        public long TreesSprayed { get; }
    }
}