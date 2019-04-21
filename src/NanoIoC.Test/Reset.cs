using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace NanoIoC.Tests
{
	public class Reset
	{
		[Fact]
		public void ResettingShouldClearAllStores()
		{
			var container = new Container();

			container.Register<TestInterface, TestClass1>(ServiceLifetime.Transient);

			container.Register<TestInterface, TestClass1>(ServiceLifetime.Scoped);
			container.Inject<TestInterface>(new TestClass1(), ServiceLifetime.Scoped);

			container.Register<TestInterface, TestClass1>(ServiceLifetime.Singleton);
			container.Inject<TestInterface>(new TestClass1(), ServiceLifetime.Singleton);
			try
			{
				container.Reset();
			}
			catch (Exception e)
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