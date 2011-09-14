using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

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

		public static void Inject<T>(this IContainer container, T instance, Lifecycle lifeCycle = Lifecycle.Singleton)
		{
			container.Inject(instance, typeof(T), lifeCycle);
		}

		public static IEnumerable<T> ResolveAll<T>(this IContainer container)
		{
			return container.ResolveAll(typeof (T)).Cast<T>();
		}
	}
}