using System.Threading.Tasks;

namespace SprayModule.Communication.ApiServerIpc
{
    /// <summary>
    /// Used to communicate with the API server process.
    /// </summary>
    public interface IApiServerBridge
    {
        /// <summary>
        /// Tell the API server how many nozzles are available.
        /// </summary>
        Task SendNumberOfAvailableNozzlesAsync(int numberOfNozzles);

        /// <summary>
        /// Tells the server if testing the nozzles was successful.
        /// </summary>
        /// <returns></returns>
        Task SendConfirmationOfNozzlesTestAsync();

        /// <summary>
        /// Tells the server if the state was set to flying.
        /// </summary>
        /// <returns></returns>
        Task SendConfirmationOfChangingStateToFlyingAsync();

        /// <summary>
        /// Tells the server if the state was set to landing.
        /// </summary>
        /// <returns></returns>
        Task SendConfirmationOfChangingStateToLandingAsync();

        /// <summary>
        /// Read a message from the API server.
        /// </summary>
        /// <returns></returns>
        Task<string> ReceiveMessageAsync();

        /// <summary>
        /// Whether the server bridge has received data but it hasn't been read.
        /// </summary>
        bool HasUnreadMessage
        {
            get;
        }
}
}