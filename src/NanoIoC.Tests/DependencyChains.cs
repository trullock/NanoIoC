using NUnit.Framework;

namespace NanoIoC.Tests
{
	[TestFixture]
	public class DependencyChains
	{
		[Test]
		public void ShouldUseExistingInstancesForDependencies()
		{
			var container = new Container();
			container.Register<TestSingletonClass>(Lifecycle.Singleton);
			container.Register<TestTransientClass>(Lifecycle.Transient);

			var singleton = container.Resolve<TestSingletonClass>();
			var transient = container.Resolve<TestTransientClass>();
			
			Assert.AreSame(singleton, transient.Dependency);
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