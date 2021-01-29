using System.Text.Json.Serialization;

namespace WildingPines.Models
{
    /// <summary>
    /// A spray nozzle in the spray boom. Used when testing the nozzles from the user interface.
    /// </summary>
    public class Nozzle
    {
        /// <summary>
        /// Deserializer needs a parameterless constructor, then uses public setters to fill in the fields.
        /// </summary>
        public Nozzle() {}
        
        public Nozzle(int id, bool shouldBeSpraying, string displayName)
        {
            Id = id;
            ShouldBeSpraying = shouldBeSpraying;
            DisplayName = displayName;
        }

        /// <summary>
        /// An identifier for the nozzle.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Whether the spray nozzle should be spraying.
        /// NOTE: there are no guarantees about the values for this variable when actual spraying operations are
        /// happening, only when testing the nozzles from the user interface.
        /// </summary>
        public bool ShouldBeSpraying { get; set; }
        
        /// <summary>
        /// Used to display the name of the nozzle in the user interface.
        /// </summary>
        public string DisplayName { get; set; }
    }
}