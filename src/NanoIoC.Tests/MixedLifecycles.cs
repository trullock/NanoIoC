using Xunit;

namespace NanoIoC.Tests
{
    public class MixedLifecycles
    {
        [Fact]
        public void ShouldThrowWhenADependencyHasAShorterLifecycle()
        {
            var container = new Container();
            container.Register<TestSingletonClass>(Lifecycle.Singleton);
            container.Register<TestTransientClass>(Lifecycle.Transient);

			try
			{
				var instance = container.Resolve<TestSingletonClass>();
			}
			catch(ContainerException e)
			{
				Assert.Equal("Cannot create dependency `NanoIoC.Tests.MixedLifecycles+TestTransientClass, NanoIoC.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null`. It's lifecycle (Transient) is shorter than the dependee's `NanoIoC.Tests.MixedLifecycles+TestSingletonClass, NanoIoC.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null` (Singleton)\nBuild Stack:\nNanoIoC.Tests.MixedLifecycles+TestSingletonClass", e.Message);
			}
        }

    	public class TestSingletonClass
        {
    		public TestSingletonClass(TestTransientClass dependency)
    		{
    		}
        }

		public class TestTransientClass
		{
			
		}
    }
}
