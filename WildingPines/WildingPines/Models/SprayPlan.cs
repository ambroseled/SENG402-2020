namespace WildingPines.Models
{
    /// <summary>
    /// A plan that contains where spray should be applied, and where it should not.
    /// A spray plan:
    /// 1. is loaded to the NUC from, say, a USB.
    /// 2. Is selected prior to flying
    /// 3. Is used during a spraying operation
    /// </summary>
    public class SprayPlan
    {
        public string Name { get; set; }

        /// <summary>
        /// This is used during deserialisation. The fields are initialised using public setters.
        /// </summary>
        public SprayPlan()
        {
        }
        public SprayPlan(string name)
        {
            Name = name;
        }
        
    }
}