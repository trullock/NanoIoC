using System;

namespace NanoIoC
{
	public abstract class OpenGenericTypeProcessor : ITypeProcessor
	{
		protected abstract Type OpenGenericTypeToClose { get; }

		public abstract Lifecycle Lifecycle { get; }

		public void Process(Type type, IContainer container)
		{
			if (!type.IsOrDerivesFrom(this.OpenGenericTypeToClose))
				return;

			if (type.IsInterface || type.IsAbstract)
				return;

			var typeArguments = type.GetGenericArgumentsClosing(this.OpenGenericTypeToClose);

			var closedType = this.OpenGenericTypeToClose.MakeGenericType(typeArguments);

			container.Register(closedType, type, this.Lifecycle);
		}
	}
}