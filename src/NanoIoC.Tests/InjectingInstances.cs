using NanoIoC;
using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class InjectingInstances
    {
        [Test]
        public void ShouldWorkForTheDefaultType()
        {
            var container = new Container();
            var testClass = new TestClass();
            container.Inject(testClass);

            var instance = container.Resolve<TestClass>();
            Assert.AreSame(testClass, instance);
        }

        [Test]
        public void ShouldWorkForAnExplicitType()
        {
            var container = new Container();
            var testClass = new TestClass();
            container.Inject<object>(testClass);

            var instance = container.Resolve<object>();
            Assert.AreSame(testClass, instance);
        }

        public class TestClass
        {
            
        }
    }
}
