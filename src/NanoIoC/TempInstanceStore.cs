using System.Collections.Generic;

namespace NanoIoC
{
	internal sealed class TempInstanceStore : SingletonInstanceStore
	{
		public TempInstanceStore(IEnumerable<object> instances)
		{
			foreach (var instance in instances)
				this.Inject(instance.GetType(), instance);
		}
	}
}