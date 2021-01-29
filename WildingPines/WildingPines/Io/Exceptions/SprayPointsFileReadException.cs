using System.IO;

namespace WildingPines.Io.Exceptions
{
    public class SprayPointsFileReadException : IOException
    {
        public SprayPointsFileReadException(string message) : base(message)
        {
        }
    }
}