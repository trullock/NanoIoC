using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class CustomCtors
    {
        [Test]
        public void ShouldResolve()
        {
            var container = new Container();

			container.Register<TestInterface>(c => new TestClass());

			var resolved = container.Resolve<TestInterface>();

            Assert.AreSame(typeof(TestClass), resolved.GetType());
        }

        [Test]
        public void ShouldResolveNullCtors()
        {
            var container = new Container();

			container.Register<TestInterface>(c => null);

			var resolved = container.Resolve<TestClass2>();
        }

        public class TestClass : TestInterface
        {
            
        }

		public class TestClass2
		{
			public TestClass2(TestInterface face)
			{
			}
		}

		public interface TestInterface
		{
			
		}
    }
}
