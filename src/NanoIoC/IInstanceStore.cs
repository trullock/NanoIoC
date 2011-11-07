using System;
using System.Collections;

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
		/// Get the single instance for the given type.
		/// Will throw if there are multiple instances
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
        object GetSingleInstance(Type type);

		/// <summary>
		/// Gets all instances for the given type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IEnumerable GetAllInstances(Type type);

		/// <summary>
		/// Removes all stored instances
		/// </summary>
		void Clear();
    }
}