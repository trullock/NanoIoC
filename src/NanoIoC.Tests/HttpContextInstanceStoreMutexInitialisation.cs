using System.IO;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace NanoIoC.Tests
{
	public class HttpContextInstanceStoreMutexInitialisation
	{
		HttpContextAccessor httpContextAccessor;

		void SetupStupContext()
		{
			this.httpContextAccessor = new HttpContextAccessor();
			this.httpContextAccessor.HttpContext = new DefaultHttpContext();
		}

		[Test]
		public void MutexIsInitialisedWhenCurrentHttpContextIsNonNull()
		{
			this.SetupStupContext();
			
			var instanceStore = new HttpContextOrExecutionContextLocalInstanceStore(this.httpContextAccessor);

			Assert.IsNotNull(instanceStore.Mutex);
		}

		[TearDown]
		public void Cleanup()
		{
			this.httpContextAccessor.HttpContext = null;
		}
	}
}
