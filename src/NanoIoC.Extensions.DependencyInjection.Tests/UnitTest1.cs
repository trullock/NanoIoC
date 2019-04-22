using System;
using Microsoft.Extensions.DependencyInjection;

namespace NanoIoC.Extensions.DependencyInjection.Tests
{
	public class UnitTest1 : Microsoft.Extensions.DependencyInjection.Specification.DependencyInjectionSpecificationTests
	{
		protected override IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection)
		{
			var container = new Container();

			
			container.Register<IServiceScopeFactory, NanoIoCServiceScopeFactory>();

			var adaptor = new ServiceProviderAdaptor(container);

			container.Inject<IServiceProvider>(adaptor);


			foreach (var thing in serviceCollection)
			{
				if(thing.ImplementationType != null)
					container.Register(thing.ServiceType, thing.ImplementationType, thing.Lifetime);
				else if(thing.ImplementationFactory != null)
					container.Register(thing.ServiceType, c => thing.ImplementationFactory(adaptor), thing.Lifetime);
				else if(thing.ImplementationInstance != null)
					container.Inject(thing.ImplementationInstance, thing.ServiceType, thing.Lifetime, InjectionBehaviour.Default);
			}

			return adaptor;
		}
	}
}
