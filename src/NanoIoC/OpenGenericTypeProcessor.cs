using System;
using System.Collections.Generic;
using System.Linq;

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

			foreach (var itype in OpenGenericTypeInterfacesFor(type))
			{
				var typeArguments = itype.GetGenericArguments();

				var closedType = this.OpenGenericTypeToClose.MakeGenericType(typeArguments);

				container.Register(closedType, type, this.Lifecycle);
			}

		}

		private IEnumerable<Type> OpenGenericTypeInterfacesFor(Type type)
		{
			return type.GetInterfaces()
			           .Where(t => t.IsGenericType &&
			                       t.GetGenericTypeDefinition() == this.OpenGenericTypeToClose);
		}
	}
}