using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace NanoIoC.Tests
{
    public class MultiThreadingWithMixedServiceLifetimes
    {
    	Container container;
    	Exception ex;

    	[Fact]
        public void ShouldConstruct()
        {
            this.container = new Container();
			this.container.Register<ISingleton, Singleton>();
			this.container.Register<IScoped, Scoped>(ServiceLifetime.Scoped);

	        var singletons = Enumerable.Range(0, 100).Select(_ => new Thread(this.ResolveSingletons)).ToArray();
	        var transients = Enumerable.Range(0, 100).Select(_ => new Thread(this.ResolveScoped)).ToArray();

	        foreach (var singleton in singletons)
		        singleton.Start();
	        foreach (var transient in transients)
		        transient.Start();


	        foreach (var singleton in singletons)
		        singleton.Join();
	        foreach (var transient in transients)
		        transient.Join();

			Assert.True(true, "Didnt deadlock");
        }

		void ResolveScoped()
		{
			container.Resolve<IScoped>();
		}
		void ResolveSingletons()
		{
			container.Resolve<ISingleton>();
		}

	    public interface IScoped
	    {

	    }

	    public class Scoped : IScoped
	    {
		    public Scoped(ISingleton singleton)
		    {
		    }
	    }

		public class Singleton : ISingleton
        {
	        public Singleton()
	        {
				// be slow
				Thread.Sleep(10);
	        }
        }

		public interface ISingleton
		{
			
		}
    }
}
