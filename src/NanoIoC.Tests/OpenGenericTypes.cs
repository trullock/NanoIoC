using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class OpenGenericTypes
    {
        [Test]
        public void ShouldConstruct()
        {
            var container = new Container();
            container.Register(typeof (ITestInterface<>), typeof (TestClass<>));

            var instance = container.Resolve<ITestInterface<int>>();
            Assert.IsInstanceOf<TestClass<int>>(instance);
        }

        public interface ITestInterface<T>
        {
        }

        public class TestClass<T> : ITestInterface<T>
        {
            
        }
    }
}
