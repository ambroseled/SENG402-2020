using System;
using System.Runtime.Serialization;

namespace SprayModule.Exception
{
    /// <summary>
    /// Custom exception to be used when the step value in the matrix is found to be invalid
    /// </summary>
    [Serializable]
    public class InvalidMatrixStepException : System.Exception
    {

        public InvalidMatrixStepException()
        {
        }

        public InvalidMatrixStepException(string message) : base(message)
        {
        }

        public InvalidMatrixStepException(string message, System.Exception inner) : base(message, inner)
        {
        }

        protected InvalidMatrixStepException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}