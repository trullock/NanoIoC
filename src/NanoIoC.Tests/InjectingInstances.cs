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
            ContainerExtensions.Inject(container, new TestClass());
            container.Inject<TestClass2>(null);

            var instance = container.Resolve<TestClass3>();
        }

        [Test]
		public void ShouldWorkForTheDefaultType()
        {
            var container = new Container();
            var testClass = new TestClass();
            ContainerExtensions.Inject(container, testClass);

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

		[Test]
		public void ShouldResolveInjectedOverRegistered()
		{
			var container = new Container();

			container.Register<TestInterface, TestClass>();

			var injector = new TestClass2();
			container.Inject<TestInterface>(injector, injectionBehaviour: InjectionBehaviour.Override);

			Assert.AreSame(injector, container.Resolve<TestInterface>());
		}
		

		[Test]
		public void ShouldResolveInjectedOverRegisteredWithThreadLifecycles()
		{
			var container = new Container();

			container.Register<TestInterface, TestClass>(Lifecycle.HttpContextOrExecutionContextLocal);

			var injector = new TestClass2();
			container.Inject<TestInterface>(injector, Lifecycle.HttpContextOrExecutionContextLocal, InjectionBehaviour.Override);

			Assert.AreSame(injector, container.Resolve<TestInterface>());
		}
		

		[Test]
		public void ShouldResolveInjectedOverRegisteredWithShorterLifecycles()
		{
			var container = new Container();

			container.Register<TestInterface, TestClass>(Lifecycle.Singleton);

			var injector = new TestClass2();
			container.Inject<TestInterface>(injector, Lifecycle.HttpContextOrExecutionContextLocal, InjectionBehaviour.Override);

			Assert.AreSame(injector, container.Resolve<TestInterface>());
		}
		
		[Test]
		public void ShouldResolveInjectedOverRegisteredWithLongerLifecycles()
		{
			var container = new Container();

			container.Register<TestInterface, TestClass>(Lifecycle.HttpContextOrExecutionContextLocal);

			var injector = new TestClass2();
			container.Inject<TestInterface>(injector, Lifecycle.Singleton, InjectionBehaviour.Override);

			Assert.AreSame(injector, container.Resolve<TestInterface>());
		}
		

		[Test]
		public void ShouldResolveInjectedOverInjected()
		{
			var container = new Container();

			container.Register<TestInterface, TestClass>();

			var injector1 = new TestClass2();
			var injector2 = new TestClass2();
			container.Inject<TestInterface>(injector1);
			container.Inject<TestInterface>(injector2, injectionBehaviour: InjectionBehaviour.Override);

			Assert.AreSame(injector2, container.Resolve<TestInterface>());
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
