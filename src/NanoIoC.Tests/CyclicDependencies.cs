using NUnit.Framework;

namespace NanoIoC.Tests
{
	[TestFixture]
	public class CyclicDependencies
	{
		[Test]
		public void ShouldThrowForUnregisteredTypes()
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

		[Test]
		public void ShouldThrowForRegisteredTypes()
		{
			var container = new Container();
			container.Register<InterfaceC, ClassC>();
			container.Register<InterfaceD, ClassD>();


			try
			{
				container.Resolve<InterfaceC>();
			}
			catch (CyclicDependencyException e)
			{
				Assert.AreEqual("Cyclic dependency detected when trying to construct `NanoIoC.Tests.CyclicDependencies+ClassC, NanoIoC.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null`", e.Message);
				Assert.AreEqual(typeof(ClassC), e.DependencyStack[0]);
				Assert.AreEqual(typeof(ClassD), e.DependencyStack[1]);
				Assert.AreEqual(2, e.DependencyStack.Length);
			}
		}

		[Test]
		public void ShouldNotThrowForNonCyclingDependencies()
		{
			var container = new Container();

			var classE = container.Resolve<ClassE>();

			Assert.IsNotNull(classE);
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

		public class ClassE
		{
			public ClassE(ClassF c, ClassF c2)
			{
			}
		}

		public class ClassF
		{
			
		}

		public interface InterfaceC
		{
			
		}

		public class ClassC : InterfaceC
		{
			public ClassC(InterfaceD d)
			{
			}
		}

		public interface InterfaceD
		{
			
		}

		public class ClassD : InterfaceD
		{
			public ClassD(InterfaceC c)
			{
			}
		}
	}
}