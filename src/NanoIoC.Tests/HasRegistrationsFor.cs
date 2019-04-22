using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace NanoIoC.Tests
{
	public class HasRegistrationsFor
	{
		[Fact]
		public void RegisteredTransient()
		{
			RegisteredServiceLifetime(ServiceLifetime.Singleton);
		}

		[Fact]
		public void RegisteredSingleton()
		{
			RegisteredServiceLifetime(ServiceLifetime.Singleton);
		}

		[Fact]
		public void RegisteredThreadLocal()
		{
			RegisteredServiceLifetime(ServiceLifetime.Scoped);
		}

		[Fact]
		public void RegisteredExecutionContextLocal()
		{
			RegisteredServiceLifetime(ServiceLifetime.Scoped);
		}

		[Fact]
		public void InjectedSingleton()
		{
			InjectedServiceLifetime(ServiceLifetime.Singleton);
		}

		[Fact]
		public void InjectedThreadLocal()
		{
			InjectedServiceLifetime(ServiceLifetime.Scoped);
		}

		[Fact]
		public void InjectedExecutionContextLocal()
		{
			InjectedServiceLifetime(ServiceLifetime.Scoped);
		}

		static void InjectedServiceLifetime(ServiceLifetime serviceLifetime)
		{
			var container = new Container();

			Assert.False(container.HasRegistrationsFor(typeof(TestClass)));

			container.Inject(new TestClass(), serviceLifetime);

			Assert.True(container.HasRegistrationsFor(typeof(TestClass)));
		}

		static void RegisteredServiceLifetime(ServiceLifetime serviceLifetime)
		{
			var container = new Container();

			Assert.False(container.HasRegistrationsFor(typeof(ITestClass)));

			container.Register(typeof(ITestClass), typeof(TestClass), serviceLifetime);

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
