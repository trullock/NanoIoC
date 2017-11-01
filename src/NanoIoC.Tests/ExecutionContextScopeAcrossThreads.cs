using System.Threading.Tasks;
using Xunit;

namespace NanoIoC.Tests
{
	public class ExecutionContextScopeAcrossThreads
	{
		[Fact]
		public void ShouldConstruct()
		{
			var container = new Container();
			container.Register<TestInterface, TestClass>(Lifecycle.ExecutionContextLocal);


			TestInterface instance = null;

			Task.Run(() => instance = container.Resolve<TestInterface>()).Wait();

			Assert.NotNull(instance);
			Assert.IsType<TestClass>(instance);
		}

		[Fact]
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

			Assert.NotNull(instance);
			Assert.Same(instance, instance2);
		}

		public class TestClass : TestInterface
		{
		}

		public interface TestInterface
		{
		}
	}
}