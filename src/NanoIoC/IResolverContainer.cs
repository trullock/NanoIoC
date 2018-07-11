using System;
using System.Collections;

namespace NanoIoC
{
	public interface IResolverContainer
	{
		/// <summary>
		/// Resolve an instance of the requested type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		object Resolve(Type type);
		IEnumerable ResolveAll(Type abstractType);
		object Resolve(Type type, params object[] dependencies);

		/// <summary>
		/// Returns a graph of the dependencies for the requested type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		GraphNode DependencyGraph(Type type);

		/// <summary>
		/// Determines if there is a registration for the given type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		bool HasRegistrationsFor(Type type);
	}
}