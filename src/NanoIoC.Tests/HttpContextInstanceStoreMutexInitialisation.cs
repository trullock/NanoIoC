using System.IO;
using System.Text;
using System.Web;
using NUnit.Framework;

namespace NanoIoC.Tests
{
	public class HttpContextInstanceStoreMutexInitialisation
	{
		[Test]
		public void MutexIsInitialisedWhenCurrentHttpContextIsNonNull()
		{
			SetupStupContext();
			var instanceStore = new HttpContextOrExecutionContextLocalInstanceStore();

			Assert.IsNotNull(instanceStore.Mutex);
		}

		[TearDown]
		public void Cleanup()
		{
			HttpContext.Current = null;
		}

		private static void SetupStupContext()
		{
			HttpContext.Current = new HttpContext(
				new HttpRequest(string.Empty, "http://localhost/", string.Empty),
				new HttpResponse(new StringWriter())
			);
		}
	}
}
