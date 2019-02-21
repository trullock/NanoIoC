using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class AsyncLocalBehaviour
    {
        static AsyncLocal<object> asyncLocal = new AsyncLocal<object>();
        static int counter;

		[Test]
        public void ShouldConstruct()
        {
			asyncLocal.Value = new object();

			var threads = Enumerable.Range(0, 100).Select(i => new Thread(this.Run)).ToArray();

			foreach (var thread in threads)
				thread.Start();
			foreach (var thread in threads)
				thread.Join();

			Assert.AreEqual(100, counter);
        }

        public void Run()
        {
	        lock (asyncLocal.Value)
		        counter++;
        }
    }
}
