using Xunit;

namespace NanoIoC.Tests
{
    public class GettingTheContainer
    {
        [Fact]
        public void TheContainShouldBeInsideItself()
        {
            var container = new Container();

        	var resolved = container.Resolve<IContainer>();

            Assert.Same(container, resolved);
        }


		[Fact]
		public void TheContainShouldBeDependable()
		{
			var container = new Container();

			var resolved = container.Resolve<TestClass>();

			Assert.Same(container, resolved.Container);
		}

        public class TestClass
        {
        	public readonly IContainer Container;

        	public TestClass(IContainer container)
        	{
        		this.Container = container;
        	}
        }
    }
}
