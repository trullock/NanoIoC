using System.Linq;
using NUnit.Framework;

namespace NanoIoC.Tests
{
	[TestFixture]
	public class HasRegistrationsFor
	{
		[Test]
		public void RegisteredTransient()
		{
			RegisteredLifecycle(Lifecycle.Singleton);
		}

		[Test]
		public void RegisteredSingleton()
		{
			RegisteredLifecycle(Lifecycle.Singleton);
		}

		[Test]
		public void RegisteredThreadLocal()
		{
			RegisteredLifecycle(Lifecycle.ExecutionContextLocal);
		}

		[Test]
		public void RegisteredExecutionContextLocal()
		{
			RegisteredLifecycle(Lifecycle.ExecutionContextLocal);
		}

		[Test]
		public void InjectedSingleton()
		{
			InjectedLifecycle(Lifecycle.Singleton);
		}

		[Test]
		public void InjectedThreadLocal()
		{
			InjectedLifecycle(Lifecycle.ExecutionContextLocal);
		}

		[Test]
		public void InjectedExecutionContextLocal()
		{
			InjectedLifecycle(Lifecycle.ExecutionContextLocal);
		}

		static void InjectedLifecycle(Lifecycle lifecycle)
		{
			var container = new Container();

			Assert.IsFalse(container.HasRegistrationsFor(typeof(TestClass)));

			container.Inject(new TestClass(), lifecycle);

			Assert.IsTrue(container.HasRegistrationsFor(typeof(TestClass)));
		}

		static void RegisteredLifecycle(Lifecycle lifecycle)
		{
			var container = new Container();

			Assert.IsFalse(container.HasRegistrationsFor(typeof(ITestClass)));

			container.Register(typeof(ITestClass), typeof(TestClass), lifecycle);

			Assert.IsTrue(container.HasRegistrationsFor(typeof(ITestClass)));
		}

		public interface ITestClass
		{
			
		}

		public class TestClass : ITestClass
		{

		}
	}
}
