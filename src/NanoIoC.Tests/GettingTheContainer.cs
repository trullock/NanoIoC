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

        public class TestClass
        {
            
        }
    }
}
