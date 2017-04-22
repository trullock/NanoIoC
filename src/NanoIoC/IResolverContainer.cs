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
	}
}