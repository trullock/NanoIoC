using System;
using System.Collections.Generic;
using System.Linq;

namespace NanoIoC
{
	public static class ContainerExtensions
	{
		/// <summary>
		/// Registers a concrete type with a lifecycle.
		/// You should only be using this for Singleton or HttpContextOrThreadLocal lifecycles.
		/// Registering a concrete type as Transient is pointless, as you get this behaviour by default.
		/// </summary>
		/// <typeparam name="TConcrete">The concrete type to register</typeparam>
		/// <param name="container"></param>
		/// <param name="lifecycle">The lifecycle of the instance</param>
		public static void Register<TConcrete>(this IContainer container, Lifecycle lifecycle)
		{
			container.Register(typeof(TConcrete), typeof(TConcrete), lifecycle);
		}

		/// <summary>
		/// Registers a concrete type for an abstract type
		/// </summary>
		/// <typeparam name="TAbstract">The abstract type you want to resolve later</typeparam>
		/// <typeparam name="TConcrete">The concrete type implementing the abstract type</typeparam>
		/// <param name="container"></param>
		/// <param name="lifecycle"></param>
		public static void Register<TAbstract, TConcrete>(this IContainer container, Lifecycle lifecycle = Lifecycle.Singleton)
		{
			container.Register(typeof(TAbstract), typeof(TConcrete), lifecycle);
		}

		/// <summary>
		/// Resolves
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="container"></param>
		/// <returns></returns>
		public static T Resolve<T>(this IContainer container)
		{
			return (T) container.Resolve(typeof (T));
		}

		/// <summary>
		/// Injects an instance 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="container"></param>
		/// <param name="instance"></param>
		/// <param name="lifeCycle"></param>
		public static void Inject<T>(this IContainer container, T instance, Lifecycle lifeCycle = Lifecycle.Singleton)
		{
			container.Inject(instance, typeof(T), lifeCycle);
		}

		/// <summary>
		/// Resolves all registered instances of T
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="container"></param>
		/// <returns></returns>
		public static IEnumerable<T> ResolveAll<T>(this IContainer container)
		{
			return container.ResolveAll(typeof (T)).Cast<T>();
		}

		public static void RemoveAllRegistrationsAndInstancesOf<T>(this IContainer container)
		{
			container.RemoveAllRegistrationsAndInstancesOf(typeof(T));
		}

		public static bool HasRegistrationFor<T>(this IContainer container)
		{
			return container.HasRegistrationFor(typeof (T));
		}

		public static void FindAndRunAllTypeProcessors(this IContainer container)
		{
			var allTypeProcessors = GetAll<ITypeProcessor>();

			var assemblies = Assemblies.AllFromApplicationBaseDirectory(a => !a.FullName.StartsWith("System"));
			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes();
				foreach (var type in types)
				{
					foreach(var typeProcessor in allTypeProcessors)
						typeProcessor.Process(type, container);
				}
			}
		}

		public static void FindAndRunAllRegistries(this IContainer container)
		{
			var allRegistries = GetAll<IContainerRegistry>();
			foreach(var registry in allRegistries)
				registry.Register(container);
		}

		static IEnumerable<T> GetAll<T>() where T : class
		{
			var assemblies = Assemblies.AllFromApplicationBaseDirectory(a => !a.FullName.StartsWith("System"));
			foreach(var assembly in assemblies)
			{
				var types = assembly.GetTypes();
				foreach(var type in types)
				{
					if (!typeof(T).IsAssignableFrom(type))
						continue;

					if(type.IsInterface || type.IsAbstract)
						continue;

					yield return Activator.CreateInstance(type) as T;
				}
			}
		}
	}
}