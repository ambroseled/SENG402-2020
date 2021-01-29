using System.Threading.Tasks;

namespace WildingPines.Communication.Sockets
{
    /// <summary>
    /// Abstracts away communication with an external resource via a UDP socket.
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