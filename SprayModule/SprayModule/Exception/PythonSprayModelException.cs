using System.Runtime.Serialization;

namespace SprayModule.Exception
{
    public class PythonSprayModelException : System.Exception
    {
        public PythonSprayModelException()
        {
        }

        public PythonSprayModelException(string message) : base(message)
        {
        }

        public PythonSprayModelException(string message, System.Exception inner) : base(message, inner)
        {
        }

        protected PythonSprayModelException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}