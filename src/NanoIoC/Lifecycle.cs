using System;

namespace NanoIoC
{
    public enum Lifecycle
    {
        Transient = 1,
        [Obsolete("Use HttpContextOrExecutionContextLocal instead", true)]
		HttpContextOrThreadLocal = 2,
        HttpContextOrExecutionContextLocal = 3,
        Singleton = 4
    }
}