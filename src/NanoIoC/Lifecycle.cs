using System;

namespace NanoIoC
{
	public enum Lifecycle
	{
		Transient = 1,
		[Obsolete("Use ExecutionContextLocal instead", true)] HttpContextOrThreadLocal = 2,
		ExecutionContextLocal = 3,
		Singleton = 4
	}
}