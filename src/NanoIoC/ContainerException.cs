using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace NanoIoC
{
	[Serializable]
	[DebuggerDisplay("{Message} - {BuildStack}")]
    public sealed class ContainerException : Exception
    {
		public Type[] BuildStack { get; private set; }
		string buildStack;

		public override string Message
		{
			get
			{
				if(!string.IsNullOrEmpty(this.buildStack))
					return base.Message + "\nBuild Stack:\n" + this.buildStack.Trim();
				
				return base.Message;
			}
		}

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
			this.BuildStack = buildStack.Reverse().ToArray();
			this.buildStack = string.Empty;

			foreach (var t in this.BuildStack)
				this.buildStack += t.FullName + "\n";
		}
    }
}