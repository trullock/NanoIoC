using System;

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
		bool ContainsInstanceFor(Type type);

		/// <summary>
		/// Get the instance for the given type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
        object GetInstance(Type type);
    }
}