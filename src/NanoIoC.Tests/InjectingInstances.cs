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

//		[Test]
//		public void ShouldWorkWhenInjectedTwice()
//		{
//			var container = new Container();
//			var testClass1 = new TestClass();
//			var testClass2 = new TestClass();
//			container.Inject<object>(testClass1, Lifecycle.Singleton);
//			container.Inject<object>(testClass2, Lifecycle.HttpContextOrThreadLocal);
//
//			var instance = container.Resolve<object>();
//			Assert.AreSame(testClass2, instance);
//		}

        public class TestClass
        {
            
        }
    }
}
