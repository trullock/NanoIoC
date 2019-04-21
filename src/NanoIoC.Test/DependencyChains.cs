using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace NanoIoC.Tests
{
	public class DependencyChains
	{
		[Fact]
		public void ShouldUseExistingInstancesForDependencies()
		{
			var container = new Container();
			container.Register<TestSingletonClass>(ServiceLifetime.Singleton);
			container.Register<TestTransientClass>(ServiceLifetime.Transient);

			var singleton = container.Resolve<TestSingletonClass>();
			var transient = container.Resolve<TestTransientClass>();
			
			Assert.Same(singleton, transient.Dependency);
		}

		public class TestSingletonClass
		{
			
		}

		public class TestTransientClass
		{
			public readonly TestSingletonClass Dependency;

			public TestTransientClass(TestSingletonClass dependency)
			{
				this.Dependency = dependency;
			}
		}
	}
}