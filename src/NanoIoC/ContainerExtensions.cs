using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace NanoIoC
{
	public static class ContainerExtensions
	{
		static IEnumerable<Type> types;

		static void EnsureTypesLoaded()
		{
			if (types != null)
				return;

			var assemblies = Assemblies.AllFromApplicationBaseDirectory(a => !a.FullName.StartsWith("System"));

			try
			{
				types = assemblies.SelectMany(a => a.GetTypes()).ToArray();
			}
			catch (TypeInitializationException e)
			{
				if (e.InnerException is ReflectionTypeLoadException inner)
					throw new ContainerException(inner.LoaderExceptions, e);

				throw;
			}
			catch (ReflectionTypeLoadException e)
			{
				throw new ContainerException(e.LoaderExceptions, e);
			}
			catch (Exception e)
			{
				throw new ContainerException("Error discovering types", e);
			}
		}

		public static void RunAllTypeProcessors(this IContainer container)
		{
			EnsureTypesLoaded();

			var typeProcessors = container.FindAllTypeProcessors();
			container.RunTypeProcessors(typeProcessors, types);
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
			EnsureTypesLoaded();

			foreach (var type in types)
				typeProcessor.Process(type, container);
		}

		public static IEnumerable<ITypeProcessor> FindAllTypeProcessors(this IContainer container)
		{
			var typeProcessors = new List<ITypeProcessor>();

			foreach (var type in types)
			{
				if (typeof(ITypeProcessor).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract && !type.ContainsGenericParameters)
					typeProcessors.Add(Activator.CreateInstance(type) as ITypeProcessor);
			}
			
			return typeProcessors;
		}


		public static void RunAllRegistries(this IContainer container)
		{
			EnsureTypesLoaded();

			var registries = container.FindAllRegistries();

			foreach (var registry in registries)
				registry.Register(container);
		}

		public static void RunRegistries(this IContainer container, IEnumerable<IContainerRegistry> registries)
		{
			foreach(var registry in registries)
				registry.Register(container);
		}

		public static IEnumerable<IContainerRegistry> FindAllRegistries(this IContainer container)
		{
			var registries = new List<IContainerRegistry>();
			
			foreach (var type in types)
			{
				if (typeof(IContainerRegistry).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract && !type.ContainsGenericParameters)
					registries.Add(Activator.CreateInstance(type) as IContainerRegistry);
			}
			
			return registries;
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