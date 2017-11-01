using Xunit;

namespace NanoIoC.Tests
{
	public class HasRegistrationsFor
	{
		[Fact]
		public void RegisteredTransient()
		{
			RegisteredLifecycle(Lifecycle.Singleton);
		}

		[Fact]
		public void RegisteredSingleton()
		{
			RegisteredLifecycle(Lifecycle.Singleton);
		}

		[Fact]
		public void RegisteredThreadLocal()
		{
			RegisteredLifecycle(Lifecycle.ExecutionContextLocal);
		}

		[Fact]
		public void RegisteredExecutionContextLocal()
		{
			RegisteredLifecycle(Lifecycle.ExecutionContextLocal);
		}

		[Fact]
		public void InjectedSingleton()
		{
			InjectedLifecycle(Lifecycle.Singleton);
		}

		[Fact]
		public void InjectedThreadLocal()
		{
			InjectedLifecycle(Lifecycle.ExecutionContextLocal);
		}

		[Fact]
		public void InjectedExecutionContextLocal()
		{
			InjectedLifecycle(Lifecycle.ExecutionContextLocal);
		}

		static void InjectedLifecycle(Lifecycle lifecycle)
		{
			var container = new Container();

			Assert.False(container.HasRegistrationsFor(typeof(TestClass)));

			container.Inject(new TestClass(), lifecycle);

			Assert.True(container.HasRegistrationsFor(typeof(TestClass)));
		}

		static void RegisteredLifecycle(Lifecycle lifecycle)
		{
			var container = new Container();

			Assert.False(container.HasRegistrationsFor(typeof(ITestClass)));

			container.Register(typeof(ITestClass), typeof(TestClass), lifecycle);

			Assert.True(container.HasRegistrationsFor(typeof(ITestClass)));
		}

		public interface ITestClass
		{
			
		}

		public class TestClass : ITestClass
		{

		}
	}
}
