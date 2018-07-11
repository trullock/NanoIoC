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
Abstract: ClassA, Concrete: ClassA, Lifecycle: Transient
     -> Abstract: ClassB, Concrete: ClassB, Lifecycle: Transient
          -> Abstract: InterfaceC, Concrete: ClassC, Lifecycle: Singleton
               -> Abstract: IEnumerable<InterfaceMultiple>, Concrete: , Lifecycle: Transient
                    -> Abstract: InterfaceMultiple, Concrete: ClassMultipleA, Lifecycle: Singleton
                    -> Abstract: InterfaceMultiple, Concrete: ClassMultipleB, Lifecycle: Singleton
          -> Abstract: InterfaceD, Concrete: ClassD, Lifecycle: Singleton
               -> Abstract: InterfaceE, Concrete: ClassE, Lifecycle: Singleton
                    -> Abstract: InterfaceC, Concrete: ClassC, Lifecycle: Singleton
                         -> Abstract: IEnumerable<InterfaceMultiple>, Concrete: , Lifecycle: Transient
                              -> Abstract: InterfaceMultiple, Concrete: ClassMultipleA, Lifecycle: Singleton
                              -> Abstract: InterfaceMultiple, Concrete: ClassMultipleB, Lifecycle: Singleton
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