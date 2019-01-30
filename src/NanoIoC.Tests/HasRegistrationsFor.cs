using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace NanoIoC.Tests
{
	[TestFixture]
	public class HasRegistrationsFor
	{
		[Test]
		public void RegisteredTransient()
		{
			RegisteredServiceLifetime(ServiceLifetime.Singleton);
		}

		[Test]
		public void RegisteredSingleton()
		{
			RegisteredServiceLifetime(ServiceLifetime.Singleton);
		}

		[Test]
		public void RegisteredThreadLocal()
		{
			RegisteredServiceLifetime(ServiceLifetime.Scoped);
		}

		[Test]
		public void RegisteredExecutionContextLocal()
		{
			RegisteredServiceLifetime(ServiceLifetime.Scoped);
		}

		[Test]
		public void InjectedSingleton()
		{
			InjectedServiceLifetime(ServiceLifetime.Singleton);
		}

		[Test]
		public void InjectedThreadLocal()
		{
			InjectedServiceLifetime(ServiceLifetime.Scoped);
		}

		[Test]
		public void InjectedExecutionContextLocal()
		{
			InjectedServiceLifetime(ServiceLifetime.Scoped);
		}

		static void InjectedServiceLifetime(ServiceLifetime serviceLifetime)
		{
			var container = new Container();

			Assert.IsFalse(container.HasRegistrationsFor(typeof(TestClass)));

			container.Inject(new TestClass(), serviceLifetime);

			Assert.IsTrue(container.HasRegistrationsFor(typeof(TestClass)));
		}

		static void RegisteredServiceLifetime(ServiceLifetime serviceLifetime)
		{
			var container = new Container();

			Assert.IsFalse(container.HasRegistrationsFor(typeof(ITestClass)));

			container.Register(typeof(ITestClass), typeof(TestClass), serviceLifetime);

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
