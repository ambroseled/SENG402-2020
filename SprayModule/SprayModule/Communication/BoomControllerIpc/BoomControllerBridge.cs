using System.Threading.Tasks;
using SprayModule.Communication.Sockets;

namespace SprayModule.Communication.BoomControllerIpc
{
    /// <inheritdoc cref="IBoomControllerBridge"/>
    public class BoomControllerBridge : IBoomControllerBridge
    {
        private const int ArduinoSocketPortLocal = 8888;
        private const int ArduinoSocketPortRemote = 8888;
        private const string ArduinoAddress = "192.168.1.24";
        
        private readonly IUdpSocket _boomSocket = new UdpSocket(ArduinoSocketPortLocal, ArduinoSocketPortRemote, ArduinoAddress);
        
        public Task UpdateNozzleStatuses(string statuses) => _boomSocket.SendAsync(statuses);
    }
}