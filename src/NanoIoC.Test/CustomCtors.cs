using System.Linq;
using Xunit;

namespace NanoIoC.Tests
{
    public class CustomCtors
    {
        [Fact]
        public void ShouldResolve()
        {
            var container = new Container();

			container.Register<TestInterface>(c => new TestClass());

			var resolved = container.Resolve<TestInterface>();

            Assert.Same(typeof(TestClass), resolved.GetType());
        }

        [Fact]
        public void ShouldResolveSameInstance()
        {
            var container = new Container();

			container.Register<TestInterface>(c => new TestClass());

			var resolved1 = container.Resolve<TestInterface>();
			var resolved2 = container.Resolve<TestInterface>();
			
			var resolvedAll = container.ResolveAll<TestInterface>().ToArray();
			Assert.Equal(1, resolvedAll.Length);

            Assert.Same(resolved1, resolved2);
        }

        [Fact]
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
