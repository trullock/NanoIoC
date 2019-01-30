using System;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace NanoIoC.Tests
{
	[TestFixture]
	public class ConstructionException
	{
		[Test]
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
				Assert.AreEqual(typeof(ClassC), e.BuildStack[2]);
				Assert.AreEqual(typeof(ClassB), e.BuildStack[1]);
				Assert.AreEqual(typeof(ClassA), e.BuildStack[0]);
				Assert.AreEqual(3, e.BuildStack.Length);
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