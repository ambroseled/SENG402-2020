using System;

namespace SprayModule.Exception
{
    /// <summary>
    /// Custom exception to be used when the helicopter is not within the bounds of the spray matrix
    /// </summary>
    [Serializable]
    public class HelicopterOutOfSprayAreaException : System.Exception
    {
        public HelicopterOutOfSprayAreaException() { }

        public HelicopterOutOfSprayAreaException(string message)
            : base(message) { }

        public HelicopterOutOfSprayAreaException(string message, System.Exception inner)
            : base(message, inner) { }
    }
}