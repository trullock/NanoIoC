using System;
using Xunit;

namespace NanoIoC.Tests
{
	public class ConstructionException
	{
		[Fact]
		public void ShouldUseExistingInstancesForDependencies()
		{
			var container = new Container();
			container.Register<ClassA>(Lifecycle.Singleton);
			container.Register<ClassB>(Lifecycle.Transient);
			container.Register<ClassC>(Lifecycle.Transient);

			try
			{
				container.Resolve<ClassC>();
			}
			catch(ContainerException e)
			{
				Assert.Equal(typeof(ClassC), e.BuildStack[0]);
				Assert.Equal(typeof(ClassB), e.BuildStack[1]);
				Assert.Equal(typeof(ClassA), e.BuildStack[2]);
				Assert.Equal(3, e.BuildStack.Length);
			}
		}

		public class ClassA
		{
			public ClassA()
			{
				throw new Exception("meh");
			}
		}

		public class ClassB
		{
			public readonly ClassA Dependency;

			public ClassB(ClassA dependency)
			{
				this.Dependency = dependency;
			}
		}

		public class ClassC
		{
			readonly ClassB dependency;

			public ClassC(ClassB dependency)
			{
				this.dependency = dependency;
			}
		}
	}
}