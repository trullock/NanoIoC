using System;

namespace NanoIoC
{
	/// <summary>
	/// 
	/// </summary>
	public interface ITypeProcessor
	{
		void Process(Type type, IContainer container);
	}
}