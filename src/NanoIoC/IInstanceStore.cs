using System;
using System.Collections;
using System.Collections.Generic;

namespace NanoIoC
{
	/// <summary>
	/// Stores instances
	/// </summary>
    public interface IInstanceStore
    {
		/// <summary>
		/// Inserts an instance
		/// </summary>
		/// <param name="registration">The associated Registration</param>
		/// <param name="type"></param>
		/// <param name="instance"></param>
		void Insert(Registration registration, Type type, object instance);
        
		/// <summary>
		/// Determines if there is an instance stored of the given type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		bool ContainsInstancesFor(Type type);

		/// <summary>
		/// Gets all instances for the given type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IEnumerable GetInstances(Type type);

		/// <summary>
		/// Injects an instance into the store
		/// </summary>
		/// <param name="type"></param>
		/// <param name="instance"></param>
		/// <param name="injectionBehaviour"></param>
		void Inject(Type type, object instance, InjectionBehaviour injectionBehaviour);

		/// <summary>
		/// Removes all instances and registrations for all types
		/// </summary>
		void RemoveAllRegistrationsAndInstances();

		/// <summary>
		/// Removes all instances and registrations of the given type
		/// </summary>
		/// <param name="type"></param>
		void RemoveAllRegistrationsAndInstances(Type type);

		/// <summary>
		/// Removes all constructed or injected instances, leaving the registrations in place
		/// </summary>
		void RemoveAllInstances();

		/// <summary>
		/// Removes instances of the given type from the store in the context of the current user
		/// </summary>
		/// <param name="type"></param>
		void RemoveInstances(Type type);
		
		/// <summary>
		/// Clones the instance store
		/// </summary>
		/// <returns></returns>
		IInstanceStore Clone();

		bool ContainsRegistrationsFor(Type type);

		IEnumerable<Registration> GetRegistrationsFor(Type type);

		void AddRegistration(Registration registration);
    }
}