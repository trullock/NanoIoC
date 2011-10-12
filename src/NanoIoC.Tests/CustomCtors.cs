using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class CustomCtors
    {
        [Test]
        public void TheContainShouldBeInsideItself()
        {
            var container = new Container();

			container.Register<TestInterface>(c => new TestClass());

			var resolved = container.Resolve<TestInterface>();

            Assert.AreSame(typeof(TestClass), resolved.GetType());
        }

        public class TestClass : TestInterface
        {
            
        }
		public interface TestInterface
		{
			
		}
    }
}
