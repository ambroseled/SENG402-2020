using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SprayModule.Communication.Sockets
{
    /// <inheritdoc cref="IUdpSocket"/>
    public class UdpSocket : IUdpSocket
    {
        private readonly int _localPort;
        private readonly int _remotePort;
        private readonly string _address;
        private IPEndPoint _endPoint;
        private readonly UdpClient _client;
        private readonly Encoding _encoding = Encoding.UTF8;

        public UdpSocket(int localPort, int remotePort, string address)
        {
            _localPort = localPort;
            _remotePort = remotePort;
            _address = address;
            _endPoint = new IPEndPoint(Dns.GetHostAddresses(_address)[0], _localPort);
            _client = new UdpClient(_localPort);
        }
        
        public async Task<string> ReceiveAsync()
        {
            var result = await _client.ReceiveAsync();
            return Decode(result.Buffer);
        }
        
        public async Task SendAsync(string data)
        {
            var bytes = Encode(data);
            await _client.SendAsync(bytes, bytes.Length, _address, _remotePort);
        }

        /// <summary>
        /// Converts a byte array into a string.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private string Decode(byte[] buffer) => _encoding.GetString(buffer);

        /// <summary>
        /// Converts a string into a byte array.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private byte[] Encode(string data) => _encoding.GetBytes(data);
        
        
        public bool HasUnreadData => _client.Available > 0;
    }
}