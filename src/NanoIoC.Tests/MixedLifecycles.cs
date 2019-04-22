using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace NanoIoC.Tests
{
    public class MixedServiceLifetimes
    {
        [Fact]
        public void ShouldThrowWhenADependencyHasAShorterServiceLifetime()
        {
            var container = new Container();
            container.Register<TestSingletonClass>(ServiceLifetime.Singleton);
            container.Register<TestTransientClass>(ServiceLifetime.Transient);

			try
			{
				var instance = container.Resolve<TestSingletonClass>();
			}
			catch(ContainerException e)
			{
				Assert.Equal("Cannot create dependency `NanoIoC.Tests.MixedServiceLifetimes+TestTransientClass, NanoIoC.Tests`. It's serviceLifetime (Transient) is shorter than the dependee's `NanoIoC.Tests.MixedServiceLifetimes+TestSingletonClass, NanoIoC.Tests` (Singleton)\nBuild Stack:\nNanoIoC.Tests.MixedServiceLifetimes+TestSingletonClass", e.Message);
			}
        }

    	public class TestSingletonClass
        {
    		public TestSingletonClass(TestTransientClass dependency)
    		{
    		}
        }

		public class TestTransientClass
		{
			
		}
    }
}
