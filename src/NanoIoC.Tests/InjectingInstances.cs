using System.Linq;
using Xunit;

namespace NanoIoC.Tests
{
    public class InjectingInstances
    {
        [Fact]
		public void ShouldWorkForNullInstances()
        {
            var container = new Container();
            ContainerExtensions.Inject(container, new TestClass());
            container.Inject<TestClass2>(null);

            var instance = container.Resolve<TestClass3>();
        }

        [Fact]
		public void ShouldWorkForTheDefaultType()
        {
            var container = new Container();
            var testClass = new TestClass();
            ContainerExtensions.Inject(container, testClass);

            var instance = container.Resolve<TestClass>();
            Assert.Same(testClass, instance);
        }

        [Fact]
        public void ShouldWorkForAnExplicitType()
        {
            var container = new Container();
            var testClass = new TestClass();
            container.Inject<object>(testClass);

            var instance = container.Resolve<object>();
            Assert.Same(testClass, instance);
        }

		[Fact]
		public void ShouldResolveRegisteredAndInjected()
		{
			var container = new Container();

			container.Register<TestInterface, TestClass>();

			var testClass = new TestClass2();
			container.Inject<TestInterface>(testClass);

			var instances = container.ResolveAll<TestInterface>().ToArray();

			Assert.Equal(2, instances.Length);

			Assert.NotSame(instances[0], instances[1]);
		}

		[Fact]
		public void ShouldResolveRegisteredResolvedAndInjected()
		{
			var container = new Container();

			container.Register<TestInterface, TestClass>();
			container.Resolve<TestInterface>();

			var testClass = new TestClass2();
			container.Inject<TestInterface>(testClass);

			var instances = container.ResolveAll<TestInterface>().ToArray();

			Assert.Equal(2, instances.Length);

			Assert.NotSame(instances[0], instances[1]);
		}

		[Fact]
		public void ShouldResolveInjectedOverRegistered()
		{
			var container = new Container();

			container.Register<TestInterface, TestClass>();

			var injector = new TestClass2();
			container.Inject<TestInterface>(injector, injectionBehaviour: InjectionBehaviour.Override);

			Assert.Same(injector, container.Resolve<TestInterface>());
		}
		

		[Fact]
		public void ShouldResolveInjectedOverRegisteredWithThreadLifecycles()
		{
			var container = new Container();

			container.Register<TestInterface, TestClass>(Lifecycle.ExecutionContextLocal);

			var injector = new TestClass2();
			container.Inject<TestInterface>(injector, Lifecycle.ExecutionContextLocal, InjectionBehaviour.Override);

			Assert.Same(injector, container.Resolve<TestInterface>());
		}
		

		[Fact]
		public void ShouldResolveInjectedOverRegisteredWithShorterLifecycles()
		{
			var container = new Container();

			container.Register<TestInterface, TestClass>(Lifecycle.Singleton);

			var injector = new TestClass2();
			container.Inject<TestInterface>(injector, Lifecycle.ExecutionContextLocal, InjectionBehaviour.Override);

			Assert.Same(injector, container.Resolve<TestInterface>());
		}
		
		[Fact]
		public void ShouldResolveInjectedOverRegisteredWithLongerLifecycles()
		{
			var container = new Container();

			container.Register<TestInterface, TestClass>(Lifecycle.ExecutionContextLocal);

			var injector = new TestClass2();
			container.Inject<TestInterface>(injector, Lifecycle.Singleton, InjectionBehaviour.Override);

			Assert.Same(injector, container.Resolve<TestInterface>());
		}
		

		[Fact]
		public void ShouldResolveInjectedOverInjected()
		{
			var container = new Container();

			container.Register<TestInterface, TestClass>();

			var injector1 = new TestClass2();
			var injector2 = new TestClass2();
			container.Inject<TestInterface>(injector1);
			container.Inject<TestInterface>(injector2, injectionBehaviour: InjectionBehaviour.Override);

			Assert.Same(injector2, container.Resolve<TestInterface>());
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
