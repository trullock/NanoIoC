using Xunit;

namespace NanoIoC.Tests
{
	public class ExecutionContextInstanceStoreMutexInitialisation
	{
		[Fact]
		public void MutexIsInitialised()
		{
			var instanceStore = new ExecutionContextLocalInstanceStore();

			Assert.NotNull(instanceStore.Mutex);
		}
	}
}
