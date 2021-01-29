using System.Threading.Tasks;
using SprayModule.Communication.Sockets;

namespace SprayModule.Communication.ApiServerIpc
{
    /// <inheritdoc cref="IApiServerBridge"/>
    public class ApiServerBridge : IApiServerBridge
    {
        private const int ServerSocketPortLocal = 54321;
        private const int ServerSocketPortRemote = 60606;
        private const string ServerAddress = "localhost";
        
        private readonly IUdpSocket _serverSocket = new UdpSocket(ServerSocketPortLocal, ServerSocketPortRemote, ServerAddress);
        
        private const string TestNozzlesReply = "testnozzles:senttoboom";
        private const string SprayStateFlyReply = "updatestate:deathmodeengaged";
        private const string SprayStateLandReply = "updatestate:deathmodedisengaged";

        public async Task SendNumberOfAvailableNozzlesAsync(int numberOfNozzles)
        {
            await _serverSocket.SendAsync($"numberofnozzles:{numberOfNozzles}");
        }

        public async Task SendConfirmationOfNozzlesTestAsync()
        {
            await _serverSocket.SendAsync(TestNozzlesReply);
        }

        public async Task SendConfirmationOfChangingStateToFlyingAsync()
        {
            await _serverSocket.SendAsync(SprayStateFlyReply);
        }

        public async Task SendConfirmationOfChangingStateToLandingAsync()
        {
            await _serverSocket.SendAsync(SprayStateLandReply);
        }

        public Task<string> ReceiveMessageAsync() => _serverSocket.ReceiveAsync();

        public bool HasUnreadMessage => _serverSocket.HasUnreadData;
    }
}