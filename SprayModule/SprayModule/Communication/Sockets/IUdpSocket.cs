using System.Threading.Tasks;

namespace SprayModule.Communication.Sockets
{
    /// <summary>
    /// An abstraction on a UDP socket.
    /// </summary>
    public interface IUdpSocket
    {
        /// <summary>
        /// Receive data asynchronously from the socket.
        /// </summary>
        /// <returns></returns>
        Task<string> ReceiveAsync();

        /// <summary>
        /// Send data asynchronously from the socket.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task SendAsync(string data);

        /// <summary>
        /// Returns whether the socket has unread data.
        /// </summary>
        bool HasUnreadData
        {
            get;
        }
    }
}