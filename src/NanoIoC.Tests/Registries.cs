using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class Registries
    {
        [Test]
        public void ShouldConstruct()
        {
            var container = new Container();

        	container.FindAndRunAllRegistries();

        	var instance = container.Resolve<TestInterface>();
			Assert.AreEqual(typeof(TestClass1), instance.GetType());
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
