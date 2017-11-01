using Xunit;

namespace NanoIoC.Tests
{
	public class HttpContextInstanceStoreMutexInitialisation
	{
		[Fact]
		public void MutexIsInitialisedWhenCurrentHttpContextIsNonNull()
		{
			var instanceStore = new ExecutionContextLocalInstanceStore();

			Assert.NotNull(instanceStore.Mutex);
		}
	}
}
