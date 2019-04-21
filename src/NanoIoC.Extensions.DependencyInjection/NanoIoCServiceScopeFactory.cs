using Microsoft.Extensions.DependencyInjection;

namespace NanoIoC.Extensions.DependencyInjection
{
	public class NanoIoCServiceScopeFactory : IServiceScopeFactory
	{
		readonly IContainer container;

		public NanoIoCServiceScopeFactory(IContainer container)
		{
			this.container = container;
		}

		public IServiceScope CreateScope()
		{
			return new NanoIoCServiceScope(container);
		}
	}
}