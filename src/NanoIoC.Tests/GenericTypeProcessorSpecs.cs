using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
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
			protected override Type OpenGenericTypeToClose => typeof (ITestInterface<>);

			public override ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
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
