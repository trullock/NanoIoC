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
            container.Register<TestSingletonClass>();
            container.Register<TestTransientClass>(Lifecycle.Transient);

			try
			{
				var instance = container.Resolve<TestSingletonClass>();
			}
			catch(ContainerException e)
			{
				Assert.AreEqual("foo", e.Message);
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
