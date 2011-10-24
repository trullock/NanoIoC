using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class RegisteredConcreteTypesWithThreadLocalScope
    {
        [Test]
        public void ShouldConstruct()
        {
            var container = new Container();
			container.Register<TestInterface, TestClass>(Lifecycle.HttpContextOrThreadLocal);

			var instance = container.Resolve<TestInterface>();
            Assert.IsInstanceOf<TestClass>(instance);
        }

		[Test]
		public void ShouldAlwaysBeTheSameInstance()
		{
			var container = new Container();
			container.Register<TestInterface, TestClass>();

			var instance = container.Resolve<TestInterface>();
			var instance2 = container.Resolve<TestInterface>();
			Assert.AreSame(instance, instance2);
		}

    	public class TestClass : TestInterface
        {
            
        }

		public interface TestInterface
		{
			
		}
    }
}
