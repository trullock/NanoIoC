using System;
using System.Runtime.Serialization;

namespace NanoIoC
{
	[Serializable]
    public class ContainerException : Exception
    {
        internal ContainerException(string message) : base(message)
        {
        }

        internal ContainerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        internal ContainerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}