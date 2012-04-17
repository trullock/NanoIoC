using System;
using System.Collections;
using System.Collections.Generic;

namespace NanoIoC
{
	/// <summary>
	/// Stores instances
	/// </summary>
    internal interface IInstanceStore
    {
		/// <summary>
		/// Inserts an instance
		/// </summary>
		/// <param name="type"></param>
		/// <param name="instance"></param>
        void Insert(Type type, object instance);
        
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
		/// Removes all stored instances
		/// </summary>
		void Clear();

		/// <summary>
		/// The underlying data structure.
		/// </summary>
		IDictionary<Type, IList<object>> Store { get; }

		/// <summary>
		/// Stores registrations for injected instances
		/// </summary>
		IDictionary<Type, IList<Registration>> InjectedRegistrations { get; }

		void Inject(Type type, object instance);
    }
}