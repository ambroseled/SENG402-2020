using System.IO;

namespace WildingPines.Io.Exceptions
{
    /// <summary>
    /// Used when trying to read or write to a USB but it is not found.
    /// </summary>
    public class UsbNotFoundException : IOException
    {
        public UsbNotFoundException(string message) : base(message)
        {
        }
    }
}
