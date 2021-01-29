using System.Collections.Generic;
using System.Threading.Tasks;
using WildingPines.Models;

namespace WildingPines.Communication.SprayModuleIpc
{
    /// <summary>
    /// Used to communicate with the spray module process.
    /// </summary>
    public interface ISprayModuleBridge
    {
        /// <summary>
        /// Ask the spray module what spray nozzles are available.
        /// </summary>
        /// <returns></returns>
        Task<List<Nozzle>> GetAvailableNozzlesAsync();

        /// <summary>
        /// Process requests to update the status of the nozzles. This may be used while testing the spray nozzles
        /// from the user interface.
        /// </summary>
        /// <param name="patchedNozzles">The nozzles with their new status.</param>
        Task UpdateNozzleStatusesAsync(List<Nozzle> patchedNozzles);

        /// <summary>
        /// Asks the spray module to change its state to be ready for flying.
        /// </summary>
        /// <param name="sprayPlanFilePath">the absolute path for the spray plan</param>
        /// <returns>Whether the operation succeeded.</returns>
        Task<bool> SetSprayModuleToFlyingState(string sprayPlanFilePath);
        
        /// <summary>
        /// Asks the spray module to change its state to be landed.
        /// </summary>
        /// <returns>Whether the operation succeeded.</returns>
        Task<bool> SetSprayModuleToLandedState();
    }
}