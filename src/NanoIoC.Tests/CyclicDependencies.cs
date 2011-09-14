using NUnit.Framework;

namespace NanoIoC.Tests
{
	[TestFixture]
	public class CyclicDependencies
	{
		[Test]
		public void ShouldThrow()
		{
			var container = new Container();

			try
			{
				container.Resolve<ClassA>();
			}
			catch(CyclicDependencyException e)
			{
				Assert.AreEqual("Cyclic dependency detected when trying to construct `NanoIoC.Tests.CyclicDependencies+ClassA, NanoIoC.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null`", e.Message);
				Assert.AreEqual(typeof (ClassA), e.DependencyStack[0]);
				Assert.AreEqual(typeof (ClassB), e.DependencyStack[1]);
				Assert.AreEqual(2, e.DependencyStack.Length);
			}
		}

		public class ClassA
		{
			public ClassA(ClassB b)
			{
			}
		}

		public class ClassB
		{
			public ClassB(ClassA a)
			{
			}
		}
	}
}