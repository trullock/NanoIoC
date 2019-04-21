using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace NanoIoC.Tests
{
    public class RegisteredConcreteTypesWithThreadLocalScope
    {
        [Fact]
        public void ShouldConstruct()
        {
            var container = new Container();
			container.Register<TestInterface, TestClass>(ServiceLifetime.Scoped);

			var instance = container.Resolve<TestInterface>();
            Assert.IsType<TestClass>(instance);
        }

		[Fact]
		public void ShouldAlwaysBeTheSameInstance()
		{
			var container = new Container();
			container.Register<TestInterface, TestClass>();

			var instance = container.Resolve<TestInterface>();
			var instance2 = container.Resolve<TestInterface>();
			Assert.Same(instance, instance2);
		}

    	public class TestClass : TestInterface
        {
            
        }

		public interface TestInterface
		{
			
		}
    }
}
