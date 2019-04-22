using Xunit;

namespace NanoIoC.Tests
{
    public class RegisteredConcreteTypes
    {
        [Fact]
        public void ShouldConstruct()
        {
            var container = new Container();
            container.Register<object, TestClass>();

            var instance = container.Resolve<object>();
            Assert.IsType<TestClass>(instance);
        }

		[Fact]
		public void ShouldAlwaysBeTheSameInstance()
		{
			var container = new Container();
			container.Register<object, TestClass>();

			var instance = container.Resolve<object>();
			var instance2 = container.Resolve<object>();
			Assert.Same(instance, instance2);
		}

		[Fact]
		public void ShouldThrowWhenConcreteDoesNotImplementAbstract()
		{
			var container = new Container();

			try
			{
				container.Register(typeof(TestInterface), typeof(TestClass));
			}
			catch(ContainerException e)
			{
				Assert.Equal("Concrete type `NanoIoC.Tests.RegisteredConcreteTypes+TestClass, NanoIoC.Tests` is not assignable to abstract type `NanoIoC.Tests.RegisteredConcreteTypes+TestInterface, NanoIoC.Tests`", e.Message);
			}
		}

    	public class TestClass
        {
            
        }

		public interface TestInterface
		{
			
		}
    }
}
