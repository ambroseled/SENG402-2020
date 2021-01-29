using System.IO;

namespace WildingPines.Io.Exceptions
{
    /// <summary>
    /// Used when we were unable to find the requested file.
    /// </summary>
    public class SprayPointsFileNotFoundException : IOException
    {
        public string FileName { get; } 
        
        public SprayPointsFileNotFoundException(string fileName) : base($"Could not find spray points file at {fileName}")
        {
            FileName = fileName;
        }
    }
}