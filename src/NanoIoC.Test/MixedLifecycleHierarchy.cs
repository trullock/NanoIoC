using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace NanoIoC.Tests
{
	public class MixedServiceLifetimeHierarchy
	{
		[Fact]
		public void ShouldNotBlock()
		{
			var insideAppVeyor = Environment.GetEnvironmentVariable("APPVEYOR")?.ToLower() == "true";
			if (insideAppVeyor)
			{
				Assert.True(true, "Test not currently functional inside AppVeyor");
				return;
			}

			var container = new Container();
			container.Register<IOuter, Outer>(ServiceLifetime.Transient);
			container.Register<IInner, Inner>(ServiceLifetime.Scoped);
			container.Register<IService, Service>();

			var cde = new CountdownEvent(5);
			var tasks = Enumerable.Range(1, 5)
				.Select(_ => new Task(() =>
				{
					cde.Signal();
					cde.Wait();
					try
					{
						container.Resolve<IOuter>();
					}
					catch (Exception e)
					{
						Assert.False(true, e.Message);
					}
				}))
				.ToList();
			var sw = Stopwatch.StartNew();
			tasks.ForEach(t => t.Start());
			var waitResult = Task.WaitAll(tasks.ToArray(), 500);
			sw.Stop();

			Assert.True(waitResult, "Timed out waiting for tasks to finish");
			Assert.True(sw.ElapsedMilliseconds < 300, $"Took too long to construct hierarchy ({sw.Elapsed})");
		}

		private class Service : IService
		{
			public void Go()
			{
				Thread.Sleep(250);
			}
		}

		private interface IService
		{
			void Go();
		}

		private class Inner : IInner
		{
			private readonly IService service;

			public Inner(IService service)
			{
				this.service = service;
			}

			public void Go()
			{
				this.service.Go();
			}
		}

		private interface IInner
		{
			void Go();
		}

		private class Outer : IOuter
		{
			public Outer(IInner inner)
			{
				inner.Go();
			}
		}

		private interface IOuter
		{
		}
	}
}