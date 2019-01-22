using System;
using NUnit.Framework;

namespace NanoIoC.Tests
{
	[TestFixture]
	public class Reset
	{
		[Test]
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
			catch (Exception e)
			{
				Assert.Fail();
			}
			var hasRegistrationFor = container.HasRegistrationFor<TestInterface>();
			Assert.IsFalse(hasRegistrationFor);
		}


		public class TestClass1 : TestInterface
		{
		}

		public interface TestInterface
		{
		}
	}
}