using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace NanoIoC.Tests
{
    public class SpeedTest
    {
    	Container container;
    	Exception ex;

        int threads = 0;

		[Fact]
        public void ShouldConstruct()
        {
			this.container = new Container();
			this.container.Register<ISingleton, Singleton>();
			this.container.Register<IScoped, Scoped>(ServiceLifetime.Scoped);
			this.container.Register<ITransient, Transient>(ServiceLifetime.Transient);

	        var singletons = Enumerable.Range(0, 1000).Select(_ => new Thread(this.ResolveSingletons)).ToArray();
	        var scopeds = Enumerable.Range(0, 1000).Select(_ => new Thread(this.ResolveScoped)).ToArray();
	        var transients = Enumerable.Range(0, 1000).Select(_ => new Thread(this.ResolveTransients)).ToArray();

	        var stopwatch = new Stopwatch();
			stopwatch.Start();

	        foreach (var singleton in singletons)
		        singleton.Start();
	        foreach (var scoped in scopeds)
		        scoped.Start();
	        foreach (var transient in transients)
		        transient.Start();
			
	        foreach (var singleton in singletons)
		        singleton.Join();
	        foreach (var scoped in scopeds)
		        scoped.Join();
	        foreach (var transient in transients)
		        transient.Join();

			stopwatch.Stop();

			Assert.True(true, "Elapsed: " + stopwatch.Elapsed + ", " + this.threads);
        }

        void ResolveSingletons()
        {
	        Interlocked.Increment(ref this.threads);
	        container.Resolve<ISingleton>();
        }

        void ResolveScoped()
		{
			Interlocked.Increment(ref this.threads);
			container.Resolve<IScoped>();
		}
        void ResolveTransients()
		{
			Interlocked.Increment(ref this.threads);
			container.Resolve<ITransient>();
		}

        public interface ITransient
		{
		}
		public class Transient : ITransient
		{
			public Transient()
			{
				Thread.Sleep(100);
			}
		}

	    public interface IScoped
	    {

	    }

	    public class Scoped : IScoped
	    {
		    public Scoped(ISingleton singleton)
		    {
				Thread.Sleep(100);
		    }
	    }

		public class Singleton : ISingleton
        {
	        public Singleton()
	        {
				// be slow
				Thread.Sleep(100);
	        }
        }

		public interface ISingleton
		{
			
		}
    }
}
