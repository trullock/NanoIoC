using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class GettingTheContainer
    {
        [Test]
        public void TheContainShouldBeInsideItself()
        {
            var container = new Container();

        	var resolved = container.Resolve<IContainer>();

            Assert.AreSame(container, resolved);
        }


		[Test]
		public void TheContainShouldBeDependable()
		{
			var container = new Container();

			var resolved = container.Resolve<TestClass>();

			Assert.AreSame(container, resolved.Container);
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
