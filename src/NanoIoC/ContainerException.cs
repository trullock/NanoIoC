using System;
using System.Runtime.Serialization;

namespace NanoIoC
{
    public sealed class ContainerException : Exception
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