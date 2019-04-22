using System;
using System.Collections.Generic;

namespace NanoIoC.Extensions.DependencyInjection
{
	public sealed class ServiceProviderAdaptor : IServiceProvider
	{
		readonly IResolverContainer container;

		public ServiceProviderAdaptor(IResolverContainer container)
		{
			this.container = container;
		}

		public object GetService(Type serviceType)
		{
			if (serviceType.IsInterface && serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
			{
				var type = serviceType.GetGenericArguments()[0];
				try
				{
					return this.container.ResolveAll(type);
				}
				catch (ContainerException e)
				{
					// TODO: logging?
					return Array.CreateInstance(type, 0);
				}
			}

			try
			{
				return this.container.Resolve(serviceType);
			}
			catch (ContainerException e)
			{
				// TODO: logging?
				return null;
			}
		}
	}
}