using System;
using System.Collections.Generic;

namespace NanoIoC
{
    public interface IContainer : IResolverContainer
    {
		/// <summary>
		/// Gets the registered concrete types for the requested type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
        IEnumerable<Registration> GetRegistrationsFor(Type type);

		/// <summary>
		/// Registers the given concrete type against the given abstract type with the given lifecycle
		/// </summary>
		/// <param name="abstractType"></param>
		/// <param name="concreteType"></param>
		/// <param name="lifecycle"></param>
        void Register(Type abstractType, Type concreteType, Lifecycle lifecycle);

		void Register(Type abstractType, Func<IResolverContainer, object> ctor, Lifecycle lifecycle);

	    /// <summary>
	    /// Injects the given instance as the given type with the given lifecycle
	    /// </summary>
	    /// <param name="instance"></param>
	    /// <param name="type"></param>
	    /// <param name="lifecycle">Must not be Transient</param>
	    /// <param name="injectionBehaviour"></param>
	    void Inject(object instance, Type type, Lifecycle lifecycle, InjectionBehaviour injectionBehaviour);

		void RemoveAllRegistrationsAndInstancesOf(Type type);

		/// <summary>
		/// Removes all instances with the given lifecycle
		/// </summary>
		/// <param name="lifecycle"></param>
		void RemoveAllInstancesWithLifecycle(Lifecycle lifecycle);

		// Empty the container of everything! (apart from itself)
    	void Reset();

	    void RemoveInstancesOf(Type type, Lifecycle lifecycle);
    }
}