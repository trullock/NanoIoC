using System;
using System.Threading;
using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class MultiThreadingWithClearing
    {
    	Container container;
    	Exception ex;

    	[Test]
        public void ShouldConstruct()
        {
            this.container = new Container();
			this.container.Register<TestInterface, TestClass>(Lifecycle.HttpContextOrExecutionContextLocal);

			for (var i = 0; i < 10; i++)
				ThreadPool.QueueUserWorkItem(this.Test);

			Thread.Sleep(1000);

           Assert.IsNull(ex);
        }

		void Test(object state)
		{
			try
			{
				container.Resolve<TestInterface>();
				container.RemoveAllInstancesWithLifecycle(Lifecycle.HttpContextOrExecutionContextLocal);
			}
			catch(Exception e)
			{
				ex = e;
			}
		}

		public class TestClass : TestInterface
        {
            
        }

		public interface TestInterface
		{
			
		}
    }
}
