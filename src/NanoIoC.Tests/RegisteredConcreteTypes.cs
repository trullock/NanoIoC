using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class RegisteredConcreteTypes
    {
        [Test]
        public void ShouldConstruct()
        {
            var container = new Container();
            container.Register<object, TestClass>();

            var instance = container.Resolve<object>();
            Assert.IsInstanceOf<TestClass>(instance);
        }

		[Test]
		public void ShouldAlwaysBeTheSameInstance()
		{
			var container = new Container();
			container.Register<object, TestClass>();

			var instance = container.Resolve<object>();
			var instance2 = container.Resolve<object>();
			Assert.AreSame(instance, instance2);
		}

		[Test]
		public void ShouldThrowWhenConcreteDoesNotImplementAbstract()
		{
			var container = new Container();

			try
			{
				container.Register(typeof(TestInterface), typeof(TestClass));
			}
			catch(ContainerException e)
			{
				Assert.AreEqual("Concrete type `NanoIoC.Tests.RegisteredConcreteTypes+TestClass, NanoIoC.Tests` is not assignable to abstract type `NanoIoC.Tests.RegisteredConcreteTypes+TestInterface, NanoIoC.Tests`", e.Message);
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
