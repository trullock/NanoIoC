using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace NanoIoC.Tests
{
	[TestFixture]
	public class ExecutionContextScopeAcrossThreads
	{
		[Test]
		public void ShouldConstruct()
		{
			var container = new Container();
			container.Register<TestInterface, TestClass>(ServiceLifetime.Scoped);


			TestInterface instance = null;

			Task.Run(() => instance = container.Resolve<TestInterface>()).Wait();

			Assert.IsNotNull(instance);
			Assert.IsInstanceOf<TestClass>(instance);
		}

		[Test]
		public void ShouldAlwaysBeTheSameInstance()
		{
			var container = new Container();
			container.Register<TestInterface, TestClass>();

			TestInterface instance = null;
			TestInterface instance2 = null;

			Task.Run(() =>
			{
				instance = container.Resolve<TestInterface>();
				instance2 = container.Resolve<TestInterface>();
			}).Wait();

			Assert.IsNotNull(instance);
			Assert.AreSame(instance, instance2);
		}

		public class TestClass : TestInterface
		{
		}

		public interface TestInterface
		{
		}
	}
}