using System;
using System.Collections;
using System.Collections.Generic;

namespace NanoIoC
{
	public interface IResolverContainer : IServiceProvider
	{
		/// <summary>
		/// Resolve an instance of the requested type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		object Resolve(Type type);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="dependencies"></param>
		/// <returns></returns>
		object Resolve(Type type, params object[] dependencies);

		/// <summary>
		/// Resolves
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T Resolve<T>();

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dependencies"></param>
		/// <returns></returns>
		object Resolve<T>(params object[] dependencies);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="abstractType"></param>
		/// <returns></returns>
		IEnumerable ResolveAll(Type abstractType);

		/// <summary>
		/// Resolves all registered instances of T
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IEnumerable<T> ResolveAll<T>();

		/// <summary>
		/// Returns a graph of the dependencies for the requested type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		GraphNode DependencyGraph(Type type);

		/// <summary>
		/// Returns the dependency graph for the given type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		GraphNode DependencyGraph<T>();

		/// <summary>
		/// Determines if there is a registration for the given type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		bool HasRegistrationsFor(Type type);

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		bool HasRegistrationFor<T>();

		/// <summary>
		/// Gets the registered concrete types for the requested type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IEnumerable<Registration> GetRegistrationsFor(Type type);
	}
}