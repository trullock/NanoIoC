using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class UnregisteredConcreteTypes
    {
        [Test]
        public void ShouldConstruct()
        {
            var container = new Container();
            var instance = container.Resolve<TestClass>();
            Assert.IsInstanceOf<TestClass>(instance);
        }

        public class TestClass
        {
            
        }
    }
}
