using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class MultipleRegistrations
    {
		[Test]
		public void ShouldThrowWhenNoRegistrations()
		{
			var container = new Container();

			try
			{
				container.ResolveAll<InterfaceA>().ToArray();
			}
			catch(ContainerException e)
			{
				Assert.AreEqual("No types registered for `NanoIoC.Tests.MultipleRegistrations+InterfaceA, NanoIoC.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null`", e.Message);
			}
		}

        [Test]
        public void ShouldResolveAll()
        {
            var container = new Container();
        	container.Register<InterfaceA, ClassA1>();
        	container.Register<InterfaceA, ClassA2>();

        	var all = container.ResolveAll<InterfaceA>().ToArray();

			Assert.AreEqual(2, all.Length);
			Assert.IsInstanceOf<ClassA1>(all[0]);
			Assert.IsInstanceOf<ClassA2>(all[1]);
        }

		[Test]
		public void ShouldInjectAll()
		{
			var container = new Container();
			container.Register<InterfaceA, ClassA1>();
			container.Register<InterfaceA, ClassA2>();

			var b = container.Resolve<ClassB>();

			Assert.AreEqual(2, b.As.Length);
			Assert.IsInstanceOf<ClassA1>(b.As[0]);
			Assert.IsInstanceOf<ClassA2>(b.As[1]);
		}

		[Test]
		public void ShouldReturnEmptyEnumerable()
		{
			var container = new Container();

			var xs = container.ResolveAll<InterfaceA>();

			Assert.AreEqual(0, xs.Count());
		}

		public interface InterfaceA
		{
			
		}

        public class ClassA1 : InterfaceA
        {
        }

		public class ClassA2 : InterfaceA
		{
		}

		public class ClassB
		{
			public readonly InterfaceA[] As;

			public ClassB(IEnumerable<InterfaceA> @as)
			{
				this.As = @as.ToArray();
			}
		}
    }
}
