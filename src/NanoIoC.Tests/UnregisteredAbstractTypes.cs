using Xunit;

namespace NanoIoC.Tests
{
    public class UnregisteredAbstractTypes
    {
        [Fact]
        public void ShouldThrow()
        {
            var container = new Container();
			try
			{
				container.Resolve<TestInterface>();
			}
			catch(ContainerException e)
			{
				Assert.Equal("Cannot resolve `NanoIoC.Tests.UnregisteredAbstractTypes+TestInterface`, it is not constructable and has no associated registration.", e.Message);	
			}
        }

        public interface TestInterface
        {
        	
        }
    }
}
