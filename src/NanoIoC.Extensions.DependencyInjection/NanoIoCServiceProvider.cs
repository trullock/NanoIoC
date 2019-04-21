using System;
using Microsoft.Extensions.DependencyInjection;

namespace NanoIoC.Extensions.DependencyInjection
{
	sealed class NanoIoCServiceProvider : IServiceProvider, ISupportRequiredService
	{
		readonly IResolverContainer container;

		public NanoIoCServiceProvider(IResolverContainer container)
		{
			this.container = container;
		}

		public object GetService(Type serviceType)
		{
			return this.container.Resolve(serviceType);
		}

		public object GetRequiredService(Type serviceType)
		{
			return this.container.Resolve(serviceType);
		}
	}
}
