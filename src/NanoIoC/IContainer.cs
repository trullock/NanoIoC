using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace NanoIoC
{
    public interface IContainer : IResolverContainer, IServiceProvider
    {
		/// <summary>
		/// Gets the registered concrete types for the requested type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
        IEnumerable<Registration> GetRegistrationsFor(Type type);

		Func<IDictionary> HttpContextItemsGetter { get; set; }

		/// <summary>
		/// Registers the given concrete type against the given abstract type with the given serviceLifetime
		/// </summary>
		/// <param name="abstractType"></param>
		/// <param name="concreteType"></param>
		/// <param name="lifetime"></param>
        void Register(Type abstractType, Type concreteType, ServiceLifetime lifetime);

		void Register(Type abstractType, Func<IResolverContainer, object> ctor, ServiceLifetime lifetime);

	    /// <summary>
	    /// Injects the given instance as the given type with the given serviceLifetime
	    /// </summary>
	    /// <param name="instance"></param>
	    /// <param name="type"></param>
	    /// <param name="lifetime">Must not be Transient</param>
	    /// <param name="injectionBehaviour"></param>
	    void Inject(object instance, Type type, ServiceLifetime lifetime, InjectionBehaviour injectionBehaviour);

		void RemoveAllRegistrationsAndInstancesOf(Type type);

		/// <summary>
		/// Removes all instances with the given serviceLifetime
		/// </summary>
		/// <param name="serviceLifetime"></param>
		void RemoveAllInstancesWithServiceLifetime(ServiceLifetime serviceLifetime);

		// Empty the container of everything! (apart from itself)
    	void Reset();

	    void RemoveInstancesOf(Type type, ServiceLifetime serviceLifetime);
    }
}