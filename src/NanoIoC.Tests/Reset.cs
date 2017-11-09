using System;
using Xunit;

namespace NanoIoC.Tests
{
	public class Reset
	{
		[Fact]
		public void ResettingShouldClearAllStores()
		{
			var container = new Container();

			container.Register<TestInterface, TestClass1>(Lifecycle.Transient);

			container.Register<TestInterface, TestClass1>(Lifecycle.ExecutionContextLocal);
			container.Inject<TestInterface>(new TestClass1(), Lifecycle.ExecutionContextLocal);

			container.Register<TestInterface, TestClass1>(Lifecycle.Singleton);
			container.Inject<TestInterface>(new TestClass1(), Lifecycle.Singleton);
			try
			{
				container.Reset();
			}
			catch (Exception)
			{
				Assert.False(true);
			}
			var hasRegistrationFor = container.HasRegistrationFor<TestInterface>();
			Assert.False(hasRegistrationFor);
		}


		public class TestClass1 : TestInterface
		{
		}

		public interface TestInterface
		{
		}
	}
}
