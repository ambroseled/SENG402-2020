using System;
using System.Threading.Tasks;
using SprayModule.Communication.Sockets;
using SprayModule.Model;

namespace SprayModule.Communication.GpsIpc
{
    /// <inheritdoc cref="IGpsBridge"/>
    public class GpsBridge : IGpsBridge
    {
        private const int GpsSocketPortLocal = 30456;
        private const int GpsSocketPortRemote = 42065;
        private const string GpsAddress = "localhost";
        
        private readonly IUdpSocket _gpsSocket = new UdpSocket(GpsSocketPortLocal, GpsSocketPortRemote, GpsAddress);
        
        public async Task<(GpsLocation, double)> GetNextGpsLocation()
        {
            var data = await _gpsSocket.ReceiveAsync();
            var items = data.Split(":");
            var latitude = Convert.ToDouble(items[0]);
            var longitude = Convert.ToDouble(items[1]);
            var yaw = Convert.ToDouble(items[2]);
            return (new GpsLocation(latitude, longitude), yaw);
        }

        public bool HasUnreadData => _gpsSocket.HasUnreadData;
    }
}