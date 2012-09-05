using System.Linq;
using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class InjectingInstances
    {
        [Test]
		public void ShouldWorkForNullInstances()
        {
            var container = new Container();
            container.Inject(new TestClass());
            container.Inject<TestClass2>(null);

            var instance = container.Resolve<TestClass3>();
        }

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

		[Test]
		public void ShouldResolveRegisteredAndInjected()
		{
			var container = new Container();

			container.Register<TestInterface, TestClass>();

			var testClass = new TestClass2();
			container.Inject<TestInterface>(testClass);

			var instances = container.ResolveAll<TestInterface>().ToArray();

			Assert.AreEqual(2, instances.Length);

			Assert.AreNotSame(instances[0], instances[1]);
		}

		[Test]
		public void ShouldResolveRegisteredResolvedAndInjected()
		{
			var container = new Container();

			container.Register<TestInterface, TestClass>();
			container.Resolve<TestInterface>();

			var testClass = new TestClass2();
			container.Inject<TestInterface>(testClass);

			var instances = container.ResolveAll<TestInterface>().ToArray();

			Assert.AreEqual(2, instances.Length);

			Assert.AreNotSame(instances[0], instances[1]);
		}


		public class TestClass : TestInterface
        {
            
        }

		public class TestClass2 : TestInterface
		{

		}

		public class TestClass3
		{
			public TestClass3(TestClass tc, TestClass2 tc2)
			{
			}
		}


		public interface TestInterface
		{
			
		}
    }
}
