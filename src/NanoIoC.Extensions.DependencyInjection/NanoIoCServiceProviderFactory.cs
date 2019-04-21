using System;
using Microsoft.Extensions.DependencyInjection;

namespace NanoIoC.Extensions.DependencyInjection
{
	public class NanoIoCServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
	{
		public ContainerBuilder CreateBuilder(IServiceCollection services)
		{
			throw new NotImplementedException();
		}

		public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
		{
			throw new NotImplementedException();
		}
	}

	public class ContainerBuilder
	{
	}
}