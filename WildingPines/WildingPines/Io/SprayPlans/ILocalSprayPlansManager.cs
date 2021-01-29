using System.Collections.Generic;
using WildingPines.Models;

namespace WildingPines.Io.SprayPlans
{
    public interface ILocalSprayPlansManager
    {

        /// <summary>
        /// Gets all the spray plans available in the NUC
        /// </summary>
        /// <returns></returns>
        public List<SprayPlan> GetSprayPlansInNuc();
    }
}