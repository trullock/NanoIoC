using System;
using Microsoft.Extensions.DependencyInjection;

namespace NanoIoC.Extensions.DependencyInjection
{
	public class NanoIoCServiceScope : IServiceScope
	{
		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public IServiceProvider ServiceProvider { get; }
	}
}