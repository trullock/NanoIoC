using Xunit;

namespace NanoIoC.Tests
{
	public class CyclicDependencies
	{
		[Fact]
		public void ShouldThrowForUnregisteredTypes()
		{
			var container = new Container();

			try
			{
				container.Resolve<ClassA>();
			}
			catch(ContainerException e)
			{
				Assert.Equal("Cyclic dependency detected when trying to construct `NanoIoC.Tests.CyclicDependencies+ClassA, NanoIoC.Tests`\nBuild Stack:\nNanoIoC.Tests.CyclicDependencies+ClassB\nNanoIoC.Tests.CyclicDependencies+ClassA", e.Message);
				Assert.Equal(typeof (ClassA), e.BuildStack[1]);
				Assert.Equal(typeof(ClassB), e.BuildStack[0]);
				Assert.Equal(2, e.BuildStack.Length);
			}
		}

		[Fact]
		public void ShouldThrowForRegisteredTypes()
		{
			var container = new Container();
			container.Register<InterfaceC, ClassC>();
			container.Register<InterfaceD, ClassD>();

			try
			{
				container.Resolve<InterfaceC>();
			}
			catch (ContainerException e)
			{
				Assert.Equal("Cyclic dependency detected when trying to construct `NanoIoC.Tests.CyclicDependencies+ClassC, NanoIoC.Tests`\nBuild Stack:\nNanoIoC.Tests.CyclicDependencies+ClassD\nNanoIoC.Tests.CyclicDependencies+ClassC", e.Message);
				Assert.Equal(typeof(ClassC), e.BuildStack[1]);
				Assert.Equal(typeof(ClassD), e.BuildStack[0]);
				Assert.Equal(2, e.BuildStack.Length);
			}
		}

		[Fact]
		public void ShouldNotThrowForNonCyclingDependencies()
		{
			var container = new Container();

			var classE = container.Resolve<ClassE>();

			Assert.NotNull(classE);
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