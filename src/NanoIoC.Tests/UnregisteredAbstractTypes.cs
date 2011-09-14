using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class UnregisteredAbstractTypes
    {
        [Test]
        public void ShouldThrow()
        {
            var container = new Container();
			try
			{
				container.Resolve<TestInterface>();
			}
			catch(ContainerException e)
			{
				Assert.AreEqual("Cannot resolve `NanoIoC.Tests.UnregisteredAbstractTypes+TestInterface`, it is not constructable and has no associated registration.", e.Message);	
			}
        }

        public interface TestInterface
        {
        	
        }
    }
}
