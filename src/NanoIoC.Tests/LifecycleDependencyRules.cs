using Xunit;

namespace NanoIoC.Tests
{
	public class LifecycleDependencyRules
	{
		[Theory]
		[InlineData(Lifecycle.Singleton, Lifecycle.ExecutionContextLocal)]
		[InlineData(Lifecycle.Singleton, Lifecycle.Transient)]
		[InlineData(Lifecycle.ExecutionContextLocal, Lifecycle.Transient)]
		public void CannotDependOnAnInstanceWithShorterLifecycle(Lifecycle dependant, Lifecycle dependency)
		{
			var container = new Container();
			container.Register<Dependant>(dependant);
			container.Register<Dependency>(dependency);

			var ex = Assert.Throws<ContainerException>(() => container.Resolve<Dependant>());
			Assert.Matches("It's lifecycle \\(.*\\) is shorter than the dependee's", ex.Message);
		}

		[Theory]
		[InlineData(Lifecycle.Singleton, Lifecycle.Singleton)]
		[InlineData(Lifecycle.ExecutionContextLocal, Lifecycle.Singleton)]
		[InlineData(Lifecycle.ExecutionContextLocal, Lifecycle.ExecutionContextLocal)]
		[InlineData(Lifecycle.Transient, Lifecycle.Singleton)]
		[InlineData(Lifecycle.Transient, Lifecycle.ExecutionContextLocal)]
		[InlineData(Lifecycle.Transient, Lifecycle.Transient)]
		public void CanDependOnAnInstanceWithLongerOrSameLifecycle(Lifecycle dependant, Lifecycle dependency)
		{
			var container = new Container();
			container.Register<Dependant>(dependant);
			container.Register<Dependency>(dependency);

			container.Resolve<Dependant>();

			Assert.True(true, "Did not throw exception");
		}

		private class Dependant { public Dependant(Dependency dependency) { } }
		private class Dependency { }
	}
}