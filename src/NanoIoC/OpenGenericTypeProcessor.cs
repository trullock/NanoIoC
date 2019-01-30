using System;
using Microsoft.Extensions.DependencyInjection;

namespace NanoIoC
{
	public abstract class OpenGenericTypeProcessor : ITypeProcessor
	{
		protected abstract Type OpenGenericTypeToClose { get; }

		public abstract ServiceLifetime ServiceLifetime { get; }

		public void Process(Type type, IContainer container)
		{
			if (!type.IsOrDerivesFrom(this.OpenGenericTypeToClose))
				return;

			if (type.IsInterface || type.IsAbstract)
				return;

			var typeArgumentsForInterfaces = type.GetGenericArgumentsClosing(this.OpenGenericTypeToClose);

			foreach (var typeArguments in typeArgumentsForInterfaces)
			{
				var closedType = this.OpenGenericTypeToClose.MakeGenericType(typeArguments);
				container.Register(closedType, type, this.ServiceLifetime);	
			}
		}
	}
}