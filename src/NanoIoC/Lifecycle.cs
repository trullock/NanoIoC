using System;

namespace NanoIoC
{
    public enum Lifecycle
    {
		Transient = 1,
		[Obsolete("Use HttpContextOrExecutionContextLocal instead")]
		HttpContextOrThreadLocal = 2,
        Singleton = 3,
		HttpContextOrExecutionContextLocal = 4,
    }
}