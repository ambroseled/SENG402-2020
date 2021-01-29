namespace WildingPines.Models
{
    /// <summary>
    /// Used to limit the amount of information returned via the API about available USB devices.
    /// </summary>
    public class UsbDrive
    {
        public UsbDrive(string name)
        {
            Name = name;
        }

        public string Name { get; }
        
    }
}