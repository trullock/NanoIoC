using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
		public static void Register<TAbstract, TConcrete>(this IContainer container, Lifecycle lifecycle = Lifecycle.Singleton) where TConcrete : TAbstract
		{
			container.Register(typeof(TAbstract), typeof(TConcrete), lifecycle);
		}

		public static void Register<TAbstract>(this IContainer container, Func<IResolverContainer, TAbstract> ctor, Lifecycle lifecycle = Lifecycle.Singleton)
		{
			container.Register(typeof(TAbstract), c => ctor(c), lifecycle);
		}
		
		/// <summary>
		/// Resolves
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="container"></param>
		/// <returns></returns>
		public static T Resolve<T>(this IResolverContainer container)
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
		/// <param name="injectionBehaviour"></param>
		public static void Inject<T>(this IContainer container, T instance, Lifecycle lifeCycle = Lifecycle.Singleton, InjectionBehaviour injectionBehaviour = InjectionBehaviour.Default)
		{
			container.Inject(instance, typeof(T), lifeCycle, injectionBehaviour);
		}
		
		/// <summary>
		/// Resolves all registered instances of T
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="container"></param>
		/// <returns></returns>
		public static IEnumerable<T> ResolveAll<T>(this IResolverContainer container)
		{
			return container.ResolveAll(typeof (T)).Cast<T>();
		}

		public static void RemoveAllRegistrationsAndInstancesOf<T>(this IContainer container)
		{
			container.RemoveAllRegistrationsAndInstancesOf(typeof(T));
		}

		public static void RemoveInstancesOf<T>(this IContainer container, Lifecycle lifecycle)
		{
			container.RemoveInstancesOf(typeof(T), lifecycle);
		}

		public static bool HasRegistrationFor<T>(this IContainer container)
		{
			return container.HasRegistrationsFor(typeof (T));
		}

		public static void RunAllTypeProcessors(this IContainer container)
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetCustomAttribute<IncludeInNanoIoCAssemblyScanningAttribute>() != null);
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
			var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetCustomAttribute<IncludeInNanoIoCAssemblyScanningAttribute>() != null);
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

			newContainer.Inject<T>(replacement, Lifecycle.ExecutionContextLocal);

			return newContainer;
		}

		public static object Resolve<T>(this IResolverContainer container, params object[] dependencies)
		{
			return container.Resolve(typeof (T), dependencies);
		}
	}
}