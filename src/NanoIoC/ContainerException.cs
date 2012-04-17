using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace NanoIoC
{
	[Serializable]
	[DebuggerDisplay("{Message} - {BuildStack}")]
    public class ContainerException : Exception
    {
		public string BuildStack { get; private set; }

		internal ContainerException(string message) : base(message)
        {
        }

		internal ContainerException(string message, Stack<Type> buildStack)
			: base(message)
		{
			StackToString(buildStack);
		}

		internal ContainerException(string message, Stack<Type> buildStack, Exception innerException)
			:this(message, innerException)
		{
			StackToString(buildStack);
		}

        internal ContainerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        internal ContainerException(string message, Exception innerException) : base(message, innerException)
        {
        }

		void StackToString(Stack<Type> buildStack)
		{
			var stack = buildStack.ToArray();
			this.BuildStack = string.Empty;

			foreach (var t in stack)
				this.BuildStack += t.FullName + "\n";
		}
    }
}