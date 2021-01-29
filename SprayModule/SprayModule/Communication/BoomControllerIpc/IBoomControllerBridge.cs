using System.Threading.Tasks;

namespace SprayModule.Communication.BoomControllerIpc
{
    /// <summary>
    /// Used to communicate with the spray boom controller.
    /// </summary>
    public interface IBoomControllerBridge
    {
        
        /// <summary>
        /// Tells the Arduino to turn on and off nozzles.
        /// </summary>
        /// <param name="statuses">
        /// A string such as "001110" that tells the Arduino which nozzles to turn
        /// on and off.
        /// </param>
        /// <returns></returns>
        Task UpdateNozzleStatuses(string statuses);
    }
}