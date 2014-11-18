using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class MultipleRegistrationsError
    {
        [Test]
        public void ShouldResolveAll()
        {
            var container = new Container();
        	container.Register<InterfaceA, ClassA1>();
        	container.Register<InterfaceA, ClassA2>();
        	container.Register<InterfaceA, ClassA1>();

        	var all = container.ResolveAll<InterfaceA>().ToArray();

			Assert.AreEqual(3, all.Length);
			Assert.IsInstanceOf<ClassA1>(all[0]);
			Assert.IsInstanceOf<ClassA2>(all[1]);
			Assert.IsInstanceOf<ClassA1>(all[2]);
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
