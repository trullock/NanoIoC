using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class SlowCtor
    {
    	Container container;

    	[Test]
        public void ShouldConstruct()
        {
            this.container = new Container();
			this.container.Register<SlowTestClass>(Lifecycle.Singleton);
			this.container.Register<FastTestClass>(Lifecycle.Singleton);

    		var resolved = new List<object>();

    		ThreadPool.QueueUserWorkItem(x => resolved.Add(this.container.Resolve<SlowTestClass>()));
			//ThreadPool.QueueUserWorkItem(x => resolved.Add(this.container.Resolve<FastTestClass>()));
			//ThreadPool.QueueUserWorkItem(x => resolved.Add(this.container.Resolve<FastTestClass>()));

    		Thread.Sleep(150);

			//Assert.AreEqual(typeof(FastTestClass), resolved[0].GetType());
			//Assert.AreEqual(typeof(FastTestClass), resolved[1].GetType());
			//Assert.AreEqual(typeof(SlowTestClass), resolved[2].GetType());
        }

		public class FastTestClass
        {
            
        }
		public class SlowTestClass
        {
			public SlowTestClass()
			{
				Thread.Sleep(100);
			}
        }
    }
}
