using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class RemovingRegistrations
    {
        [Test]
        public void ShouldntHaveRegistrations()
        {
            var container = new Container();
        	container.Register<InterfaceA, ClassA1>();
        	container.Register<InterfaceA, ClassA2>();
        	container.Inject<InterfaceA>(new ClassA2());

			container.RemoveAllRegistrationsAndInstancesOf<InterfaceA>();

        	Assert.IsFalse(container.HasRegistrationFor<InterfaceA>());
        }

		[Test]
		public void ShouldRemoveInstances()
		{
			var container = new Container();
			container.Inject<InterfaceA>(new ClassA1());
			container.Register<InterfaceA, ClassA2>();

			container.RemoveAllRegistrationsAndInstancesOf<InterfaceA>();

			Assert.AreEqual(0, container.ResolveAll<InterfaceA>().Count());
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
