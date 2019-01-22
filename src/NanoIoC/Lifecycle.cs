namespace NanoIoC
{
    public enum Lifecycle
    {
        Transient = 1,
		//HttpContextOrThreadLocal = 2,
        ExecutionContextLocal = 3,
        Singleton = 4
    }
}