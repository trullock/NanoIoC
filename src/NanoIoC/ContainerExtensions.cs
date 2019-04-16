using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace NanoIoC
{
	public static class ContainerExtensions
	{
		public static void RunAllTypeProcessors(this IContainer container)
		{
			var assemblies = Assemblies.AllFromApplicationBaseDirectory(a => !a.FullName.StartsWith("System"));
			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes();
				foreach (var type in types)
				{
					foreach(var typeProcessor in Container.TypeProcessors)
						typeProcessor.Process(type, container);
				}
			}
		}

		public static void RunTypeProcessors(this IContainer container, IEnumerable<ITypeProcessor> typeProcessors, IEnumerable<Type> types)
		{
			foreach (var type in types)
			{
				foreach(var typeProcessor in typeProcessors)
					typeProcessor.Process(type, container);
			}
		}

		public static void RunTypeProcessor(this IContainer container, ITypeProcessor typeProcessor)
		{
			var assemblies = Assemblies.AllFromApplicationBaseDirectory(a => !a.FullName.StartsWith("System"));
			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes();
				foreach (var type in types)
				{
					typeProcessor.Process(type, container);
				}
			}
		}

		public static void RunAllRegistries(this IContainer container)
		{
			foreach(var registry in Container.Registries)
				registry.Register(container);
		}

		public static void RunRegistries(this IContainer container, IEnumerable<IContainerRegistry> registries)
		{
			foreach(var registry in registries)
				registry.Register(container);
		}

		public static IContainer With<T>(this IContainer container, T replacement)
		{
			// dirty
			if(!(container is Container))
				throw new ArgumentException("container is not a `" + typeof(Container).FullName + "`");
			
			var newContainer = new Container(container as Container);

			if (newContainer.HasRegistrationFor<T>())
				newContainer.RemoveAllRegistrationsAndInstancesOf<T>();

			newContainer.Inject<T>(replacement, ServiceLifetime.Scoped);

			return newContainer;
		}
	}
}