using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NanoIoC
{
	[Serializable]
	public class CyclicDependencyException : ContainerException
	{
		public readonly Type[] DependencyStack;

		internal CyclicDependencyException(string message, Type[] dependencyStack)
			: base(message)
		{
			this.DependencyStack = dependencyStack;
		}

		internal CyclicDependencyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		internal CyclicDependencyException(string message, Type[] dependencyStack, Exception innerException) : base(message, innerException)
		{
			this.DependencyStack = dependencyStack;
		}
	}
}