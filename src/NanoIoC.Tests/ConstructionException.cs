using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace NanoIoC.Tests
{
	public class ConstructionExceptions
	{
		[Fact]
		public void ShouldUseExistingInstancesForDependencies()
		{
			var container = new Container();
			container.Register<ClassA>(ServiceLifetime.Singleton);
			container.Register<ClassB>(ServiceLifetime.Transient);
			container.Register<ClassC>(ServiceLifetime.Transient);

			try
			{
				container.Resolve<ClassC>();
			}
			catch(ContainerException e)
			{
				Assert.Equal(typeof(ClassC), e.BuildStack[2]);
				Assert.Equal(typeof(ClassB), e.BuildStack[1]);
				Assert.Equal(typeof(ClassA), e.BuildStack[0]);
				Assert.Equal(3, e.BuildStack.Length);
			}
		}

		[Fact]
		public void ShouldThrowContainerException()
		{
			var container = new Container();

			try
			{
				container.Resolve<ClassD>();
			}
			catch(ContainerException e)
			{
				
			}
			catch(Exception e)
			{
				Assert.False(true, "Shouldn't get here");
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
		public class ClassD
		{
			readonly ClassE dependency;

			public ClassD(ClassE dependency)
			{
				this.dependency = dependency;
			}
		}

		public class ClassE
		{
			public ClassE()
			{
				throw new Exception("Aren't I exceptional?");
			}
		}
	}
}