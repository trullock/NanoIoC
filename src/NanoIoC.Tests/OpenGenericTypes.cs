using Xunit;

namespace NanoIoC.Tests
{
    public class OpenGenericTypes
    {
        [Fact]
        public void ShouldConstruct()
        {
            var container = new Container();
            container.Register(typeof (ITestInterface<>), typeof (TestClass<>));

            var instance = container.Resolve<ITestInterface<int>>();
            Assert.IsType<TestClass<int>>(instance);
        }

		[Fact]
		public void ShouldConstructDependencies()
		{
			var container = new Container();
			container.Register(typeof(ITestInterface<>), typeof(TestClass<>));

			var instance = container.Resolve<TestClass>();
			Assert.NotNull(instance);
		}

		public class TestClass
		{
			public TestClass(ITestInterface<string> face)
			{
			}
		}

        public interface ITestInterface<T>
        {
        }

        public class TestClass<T> : ITestInterface<T>
        {
            
        }
    }
}
