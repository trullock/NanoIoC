using System;
using System.Threading;
using Xunit;

namespace NanoIoC.Tests
{
    public class MultiThreading
    {
    	Container container;
    	Exception ex;

    	[Fact]
        public void ShouldConstruct()
        {
            this.container = new Container();
			this.container.Register<TestInterface, TestClass>();

			for (var i = 0; i < 10; i++)
				ThreadPool.QueueUserWorkItem(this.Test);

			Thread.Sleep(1000);

           Assert.Null(ex);
        }

		void Test(object state)
		{
			try
			{
				container.Resolve<TestInterface>();
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
