using System.Threading.Tasks;
using SprayModule.Model;

namespace SprayModule.Communication.GpsIpc
{
    
    /// <summary>
    /// Used to communicate with the GPS.
    /// </summary>
    public interface IGpsBridge
    {
        /// <summary>
        /// Returns the next GPS location and yaw from the bridge.
        /// </summary>
        /// <returns></returns>
        Task<(GpsLocation, double)> GetNextGpsLocation();
        
        /// <summary>
        /// Whether the bridge has received data and it hasn't been read yet.
        /// </summary>
        bool HasUnreadData
        {
            get;
        }
    }
}