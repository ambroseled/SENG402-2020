using System;

namespace WildingPines.Models
{
    /// <summary>
    /// Used to show that the server is up and running.
    /// </summary>
    public class HealthCheck
    {
        public DateTime Timestamp { get; }
        public HealthCheck()
        {
            Timestamp = DateTime.Now;
        }
    }
}