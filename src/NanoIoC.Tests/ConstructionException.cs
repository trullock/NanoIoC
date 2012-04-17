using System;
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
			container.Register<ClassA>(Lifecycle.Singleton);
			container.Register<ClassB>(Lifecycle.Transient);
			container.Register<ClassC>(Lifecycle.Transient);

			try
			{
				container.Resolve<ClassC>();
			}
			catch(ContainerException e)
			{
				Assert.AreEqual("NanoIoC.Tests.ConstructionException+ClassA\nNanoIoC.Tests.ConstructionException+ClassC\nNanoIoC.Tests.ConstructionException+ClassB\n", e.BuildStack);
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