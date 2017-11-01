using System;
using System.Linq;
using Xunit;

namespace NanoIoC.Tests
{
    public class GenericTypeProcessorSpecs
    {
        [Fact]
        public void ShouldConstruct()
        {
            var container = new Container();
            container.RunAllTypeProcessors();

            var instances = container.ResolveAll<ITestInterface<string>>().ToArray();

        	Assert.Equal(1, instances.Length);
			Assert.Equal(typeof(TestClass), instances[0].GetType());
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
