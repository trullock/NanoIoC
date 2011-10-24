using System;
using System.Linq;
using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class GenericTypeProcessorSpecs
    {
        [Test]
        public void ShouldConstruct()
        {
            var container = new Container();
            container.RunAllTypeProcessors();

            var instances = container.ResolveAll<ITestInterface<string>>().ToArray();

        	Assert.AreEqual(1, instances.Length);
			Assert.AreEqual(typeof(TestClass), instances[0].GetType());
        }

		public class TISTypeProcessor : OpenGenericTypeProcessor
		{
			protected override Type OpenGenericTypeToClose
			{
				get { return typeof (ITestInterface<>); }
			}

			public override Lifecycle Lifecycle
			{
				get { return Lifecycle.Singleton; }
			}
		}

        public interface ITestInterface<T>
        {
        }

        public class TestClass : ITestInterface<string>
        {
            
        }

		public abstract class FailClass : ITestInterface<string>
		{
			
		}
    }
}
