using System.Linq;
using System.Threading;
using Xunit;

namespace NanoIoC.Tests
{
    public class AsyncLocalBehaviour
    {
        static AsyncLocal<object> asyncLocal = new AsyncLocal<object>();
        static int counter;

		[Fact]
        public void ShouldConstruct()
        {
			asyncLocal.Value = new object();

			var threads = Enumerable.Range(0, 100).Select(i => new Thread(this.Run)).ToArray();

			foreach (var thread in threads)
				thread.Start();
			foreach (var thread in threads)
				thread.Join();

			Assert.Equal(100, counter);
        }

        public void Run()
        {
	        lock (asyncLocal.Value)
		        counter++;
        }
    }
}
