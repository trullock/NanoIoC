using Xunit;

namespace NanoIoC.Tests
{
    public class RegisteringAbstractTypes
    {
        [Fact]
        public void ShouldThrowOnInterfaces()
        {
            var container = new Container();
			try
			{
				container.Register<object, TestInterface>();
				Assert.False(true, "Did not throw");
			}
			catch(ContainerException e)
			{
				Assert.Equal("Concrete type `" + typeof(TestInterface).GetNameForException() + "` is not a concrete type", e.Message);
			}
        }

		[Fact]
		public void ShouldThrowOnAbstract()
		{
			var container = new Container();
			try
			{
				container.Register<object, TestClass>();
				Assert.False(true, "Did not throw");
			}
			catch (ContainerException e)
			{
				Assert.Equal("Concrete type `" + typeof(TestClass).GetNameForException() + "` is not a concrete type", e.Message);
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
