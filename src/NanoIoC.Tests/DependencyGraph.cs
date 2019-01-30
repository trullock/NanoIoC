using System.Collections.Generic;
using NUnit.Framework;

namespace NanoIoC.Tests
{
	[TestFixture]
	public class DependencyGraph
	{
		[Test]
		public void GraphShouldBeCorrect()
		{
			var container = new Container();
			container.Register<InterfaceC, ClassC>();
			container.Register<InterfaceD, ClassD>();
			container.Register<InterfaceE, ClassE>();
			container.Register<InterfaceMultiple, ClassMultipleA>();
			container.Register<InterfaceMultiple, ClassMultipleB>();

			var graph = container.DependencyGraph<ClassA>();
			Assert.AreEqual(@"
Abstract: ClassA, Concrete: ClassA, ServiceLifetime: Transient
     -> Abstract: ClassB, Concrete: ClassB, ServiceLifetime: Transient
          -> Abstract: InterfaceC, Concrete: ClassC, ServiceLifetime: Singleton
               -> Abstract: IEnumerable<InterfaceMultiple>, Concrete: , ServiceLifetime: Transient
                    -> Abstract: InterfaceMultiple, Concrete: ClassMultipleA, ServiceLifetime: Singleton
                    -> Abstract: InterfaceMultiple, Concrete: ClassMultipleB, ServiceLifetime: Singleton
          -> Abstract: InterfaceD, Concrete: ClassD, ServiceLifetime: Singleton
               -> Abstract: InterfaceE, Concrete: ClassE, ServiceLifetime: Singleton
                    -> Abstract: InterfaceC, Concrete: ClassC, ServiceLifetime: Singleton
                         -> Abstract: IEnumerable<InterfaceMultiple>, Concrete: , ServiceLifetime: Transient
                              -> Abstract: InterfaceMultiple, Concrete: ClassMultipleA, ServiceLifetime: Singleton
                              -> Abstract: InterfaceMultiple, Concrete: ClassMultipleB, ServiceLifetime: Singleton
".TrimStart(), graph.ToString());
		}

		public interface InterfaceMultiple
		{

		}

		public class ClassMultipleA : InterfaceMultiple
		{
			public ClassMultipleA()
			{
			}
		}

		public class ClassMultipleB : InterfaceMultiple
		{

		}

		public interface InterfaceC
		{
			
		}

		public class ClassC : InterfaceC
		{
			public ClassC(IEnumerable<InterfaceMultiple> multiples)
			{
			}
		}

		public interface InterfaceD 
		{
			
		}

		public class ClassD : InterfaceD
		{
			public ClassD(InterfaceE e)
			{
			}
		}
		public interface InterfaceE
		{

		}

		public class ClassE : InterfaceE
		{
			public ClassE(InterfaceC c)
			{
			}
		}

		public class ClassB
		{
			public ClassB(InterfaceC c, InterfaceD d)
			{
			}
		}

		public class ClassA
		{
			public ClassA(ClassB dependency)
			{
			}
		}
	}
}