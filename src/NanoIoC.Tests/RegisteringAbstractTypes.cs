using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class RegisteringAbstractTypes
    {
        [Test]
        public void ShouldThrowOnInterfaces()
        {
            var container = new Container();
			try
			{
				container.Register<object, TestInterface>();
				Assert.Fail("Did not throw");
			}
			catch(ContainerException e)
			{
				Assert.AreEqual("Concrete type `" + typeof(TestInterface).AssemblyQualifiedName + "` is not a concrete type", e.Message);
			}
        }

		[Test]
		public void ShouldThrowOnAbstract()
		{
			var container = new Container();
			try
			{
				container.Register<object, TestClass>();
				Assert.Fail("Did not throw");
			}
			catch (ContainerException e)
			{
				Assert.AreEqual("Concrete type `" + typeof(TestClass).AssemblyQualifiedName + "` is not a concrete type", e.Message);
			}
		}

    	public abstract class TestClass
        {
            
        }

		public interface TestInterface
		{
			
		}
    }
}
