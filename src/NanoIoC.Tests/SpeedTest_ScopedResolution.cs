using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class SpeedTest_ScopedResolution
    {
    	Container container;

		[Test]
        public void ShouldConstruct()
        {
			this.container = new Container();
			this.container.Register<IScoped, Scoped>(ServiceLifetime.Scoped);
			
	        var scopeds = Enumerable.Range(0, 1000).Select(_ => new Thread(this.ResolveScoped)).ToArray();

	        var stopwatch = new Stopwatch();
			stopwatch.Start();
			
	        foreach (var scoped in scopeds)
		        scoped.Start();
			
	        foreach (var scoped in scopeds)
		        scoped.Join();

			stopwatch.Stop();

			Assert.Pass("Elapsed: " + stopwatch.Elapsed);
        }
		
        void ResolveScoped()
		{
			container.Resolve<IScoped>();
		}
		
	    public interface IScoped
	    {

	    }

	    public class Scoped : IScoped
	    {
	    }
    }
}
