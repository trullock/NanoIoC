using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class RemovingRegistrations
    {
        [Test]
        public void ShouldResolveAll()
        {
            var container = new Container();
        	container.Register<InterfaceA, ClassA1>();
        	container.Register<InterfaceA, ClassA2>();
        	container.Inject<InterfaceA>(new ClassA2());

			container.RemoveAllRegistrationsAndInstancesOf<InterfaceA>();

        	Assert.IsFalse(container.HasRegistrationFor<InterfaceA>());
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
