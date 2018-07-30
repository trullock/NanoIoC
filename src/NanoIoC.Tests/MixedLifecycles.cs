using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class MixedLifecycles
    {
        [Test]
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
				Assert.AreEqual("Cannot create dependency `NanoIoC.Tests.MixedLifecycles+TestTransientClass, NanoIoC.Tests`. It's lifecycle (Transient) is shorter than the dependee's `NanoIoC.Tests.MixedLifecycles+TestSingletonClass, NanoIoC.Tests` (Singleton)\nBuild Stack:\nNanoIoC.Tests.MixedLifecycles+TestSingletonClass", e.Message);
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
