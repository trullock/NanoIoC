
using Xunit;

namespace NanoIoC.Tests
{
    public class Registries
    {
        [Fact]
        public void ShouldConstruct()
        {
            var container = new Container();

        	container.RunAllRegistries();

        	var instance = container.Resolve<TestInterface>();
			Assert.Equal(typeof(TestClass1), instance.GetType());
        }


    	public class TestClass1 : TestInterface
        {
            
        }

		public interface TestInterface
		{
			
		}

		public class TestClassContainerRegistry : IContainerRegistry
		{
			public void Register(IContainer container)
			{
				container.Register<TestInterface, TestClass1>();
			}
		}
    }
}
