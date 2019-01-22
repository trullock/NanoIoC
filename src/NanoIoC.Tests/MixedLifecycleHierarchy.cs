using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NanoIoC.Tests
{
	public class MixedLifecycleHierarchy
	{
		[Test]
		public void ShouldNotBlock()
		{
			var insideAppVeyor = Environment.GetEnvironmentVariable("APPVEYOR")?.ToLower() == "true";
			if(insideAppVeyor)
				Assert.Ignore("Test not currently functional inside AppVeyor");

			var container = new Container();
			container.Register<IOuter, Outer>(Lifecycle.Transient);
			container.Register<IInner, Inner>(Lifecycle.ExecutionContextLocal);
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
						Assert.Fail(e.Message);
					}
				}))
				.ToList();
			var sw = Stopwatch.StartNew();
			tasks.ForEach(t => t.Start());
			var waitResult = Task.WaitAll(tasks.ToArray(), 500);
			sw.Stop();

			Assert.That(waitResult, Is.True, "Timed out waiting for tasks to finish");
			Assert.That(sw.ElapsedMilliseconds, Is.LessThan(300), "Took too long to construct hierarchy ({0})", sw.Elapsed);
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