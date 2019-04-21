using System;
using Microsoft.Extensions.DependencyInjection;

namespace NanoIoC.Extensions.DependencyInjection
{
	public class NanoIoCServiceScope : IServiceScope
	{
		readonly IResolverContainer container;

		public NanoIoCServiceScope(IResolverContainer container)
		{
			this.container = container;
		}

		public void Dispose()
		{
		}

		public IServiceProvider ServiceProvider => new ServiceProviderAdaptor(container);
	}
}