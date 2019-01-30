using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace NanoIoC.Tests
{
	[TestFixture]
	public class SlowCtor
	{
		[Test]
		public void ShouldntBlockEachOther()
		{
			var container = new Container();

			container.Register<TestInterface>(c =>
			{
				Thread.Sleep(1000);
				return new TestClass();
			}, ServiceLifetime.Transient);

			var stopwatch = new Stopwatch();
			stopwatch.Start();

			Enumerable.Range(0, 10).Select(i =>
			{
				var thread = new Thread(Construct);
				thread.Start(container);
				return thread;
			}).ToList().ForEach(t => t.Join());

			stopwatch.Stop();

			// if this takes 10s then they ran serially
			Assert.Less(stopwatch.Elapsed.TotalSeconds, 1.5);
		}

		static void Construct(object o)
		{
			var resolved = (o as IContainer).Resolve<TestInterface>();
		}

		public class TestClass : TestInterface
		{

		}

		public interface TestInterface
		{

		}
	}
}
